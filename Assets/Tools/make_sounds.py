"""Bala sound kit generator.
Makes every sound effect and both music loops from scratch (no samples),
so all files are own work. Run: python make_sounds.py
Output: out/SFX/*.wav (mono) and out/Music/*.wav (stereo, seamless loops).
"""
import os
import numpy as np
from scipy.signal import butter, sosfilt
from scipy.io import wavfile

SR = 44100
rng = np.random.default_rng(7)
OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "out")


# ---------- basic building blocks ----------
def t_axis(dur):
    return np.arange(int(dur * SR)) / SR


def env(n, attack, decay, curve=4.0):
    a = int(attack * SR)
    e = np.ones(n)
    if a > 0:
        e[:a] = np.linspace(0, 1, a)
    rest = n - a
    if rest > 0:
        e[a:] = np.exp(-curve * np.linspace(0, 1, rest) * (rest / SR) / max(decay, 1e-3))
    return e


def sweep(f0, f1, dur, kind="sine", exp=True):
    t = t_axis(dur)
    f = f0 * (f1 / f0) ** (t / dur) if exp else f0 + (f1 - f0) * t / dur
    ph = 2 * np.pi * np.cumsum(f) / SR
    if kind == "sine":
        return np.sin(ph)
    if kind == "square":
        return np.sign(np.sin(ph)) * 0.6
    if kind == "saw":
        return 2 * ((ph / (2 * np.pi)) % 1) - 1
    if kind == "tri":
        return 2 * np.abs(2 * ((ph / (2 * np.pi)) % 1) - 1) - 1


def noise(dur):
    return rng.uniform(-1, 1, int(dur * SR))


def lp(x, fc, order=2):
    return sosfilt(butter(order, min(fc, SR / 2 - 100), "low", fs=SR, output="sos"), x)


def hp(x, fc, order=2):
    return sosfilt(butter(order, fc, "high", fs=SR, output="sos"), x)


def bp(x, lo, hi, order=2):
    return sosfilt(butter(order, [lo, hi], "band", fs=SR, output="sos"), x)


def pad(x, dur):
    n = int(dur * SR)
    return np.pad(x, (0, max(0, n - len(x))))[:n] if len(x) < n else x


def mix(*parts):
    n = max(len(p) for p in parts)
    out = np.zeros(n)
    for p in parts:
        out[: len(p)] += p
    return out


def place(buf, x, at):
    i = int(at * SR)
    j = min(len(buf), i + len(x))
    buf[i:j] += x[: j - i]


def fade_out(x, dur=0.01):
    n = min(len(x), int(dur * SR))
    x = x.copy()
    x[-n:] *= np.linspace(1, 0, n)
    return x


def norm(x, peak=0.85):
    m = np.max(np.abs(x))
    return x * (peak / m) if m > 0 else x


def midi(m):
    return 440.0 * 2 ** ((m - 69) / 12)


# ---------- instruments ----------
def dombra(freq, dur, bright=0.5, vol=1.0):
    """Plucked string (Karplus-Strong) with a small wooden body."""
    n = int(dur * SR)
    period = max(2, int(round(SR / freq)))
    buf = rng.uniform(-1, 1, period)
    buf = lp(buf, 1500 + 6000 * bright)
    out = np.zeros(n)
    damp = 0.996 - 0.004 * (freq / 1000)
    for i in range(n):
        v = buf[i % period]
        out[i] = v
        buf[i % period] = damp * 0.5 * (v + buf[(i + 1) % period])
    body = bp(out, 180, 2400) * 0.6 + out * 0.4
    pick = hp(noise(0.012), 2000) * np.linspace(1, 0, int(0.012 * SR)) * 0.25
    body[: len(pick)] += pick
    return fade_out(body * vol, 0.02)


def flute(freq, dur, vol=1.0):
    """Sybyzgy-like breathy flute."""
    t = t_axis(dur)
    vib = 1 + 0.006 * np.sin(2 * np.pi * 5.2 * t) * np.clip(t / 0.3, 0, 1)
    ph = 2 * np.pi * np.cumsum(freq * vib) / SR
    tone = np.sin(ph) + 0.25 * np.sin(2 * ph) + 0.08 * np.sin(3 * ph)
    breath = bp(noise(dur), freq * 1.5, min(freq * 5, 15000)) * 0.12
    a = int(0.08 * SR)
    e = np.ones(len(t))
    e[:a] = np.linspace(0, 1, a)
    r = int(0.15 * SR)
    e[-r:] *= np.linspace(1, 0, r)
    return (tone + breath) * e * vol


def drum(dur=0.35, vol=1.0, pitch=95):
    """Dauylpaz-like frame drum: low thump + skin."""
    body = sweep(pitch * 1.8, pitch, dur) * env(int(dur * SR), 0.002, 0.18)
    skin = bp(noise(0.06), 300, 1800) * env(int(0.06 * SR), 0.001, 0.03)
    return mix(body, skin * 0.5) * vol


def shaker(vol=1.0):
    d = 0.05
    return hp(noise(d), 5000) * env(int(d * SR), 0.004, 0.02) * vol


# ---------- sound effects ----------
def sfx_jump(v):
    f0, f1 = [(260, 640), (240, 600)][v]
    d = 0.17
    x = sweep(f0, f1, d, "square") * env(int(d * SR), 0.005, 0.12)
    return lp(x, 3200)


def sfx_shoot(v):
    d = 0.2
    n = noise(d)
    whoosh = lp(n, 2500) * np.sin(np.linspace(0, np.pi, len(n))) ** 2
    click = sweep([1800, 1500][v], 900, 0.03) * env(int(0.03 * SR), 0.001, 0.01)
    return mix(click * 0.6, whoosh * 0.5)


def sfx_coin(v):
    a, b = [(83, 88), (85, 90)][v]
    x = np.zeros(int(0.45 * SR))
    place(x, dombra(midi(a), 0.2, 0.9), 0)
    place(x, dombra(midi(b), 0.4, 0.9), 0.07)
    place(x, sweep(midi(b + 12), midi(b + 12), 0.25) * env(int(0.25 * SR), 0.002, 0.1) * 0.15, 0.07)
    return x


def sfx_heal(v):
    x = np.zeros(int(0.7 * SR))
    for i, m in enumerate([74, 78, 81, 86]):
        place(x, dombra(midi(m), 0.35, 0.8), i * 0.07)
    return x


def sfx_player_hurt(v):
    d = 0.3
    thud = sweep([190, 160][v], 55, d) * env(int(d * SR), 0.002, 0.12)
    crunch = lp(noise(0.08), [2200, 1600][v]) * env(int(0.08 * SR), 0.001, 0.03)
    buzz = lp(sweep(140, 90, 0.18, "square"), 900) * env(int(0.18 * SR), 0.005, 0.1) * 0.5
    return mix(thud, crunch * 0.7, buzz)


def sfx_enemy_hit(v):
    body = [420, 360, 480][v]
    d = 0.14
    knock = sweep(body * 1.6, body * 0.6, d) * env(int(d * SR), 0.001, 0.05)
    tick = bp(noise(0.03), 1500, 5000) * env(int(0.03 * SR), 0.0005, 0.01)
    return mix(knock, tick * 0.6)


def sfx_enemy_death(v):
    d = 0.4
    fall = lp(sweep(640, 140, d, "square"), 2500) * env(int(d * SR), 0.003, 0.25)
    poof = lp(noise(0.25), 1200) * env(int(0.25 * SR), 0.01, 0.12)
    return mix(fall * 0.7, poof * 0.6)


def sfx_fox_windup(v):
    d = 0.55
    t = t_axis(d)
    base = sweep(70, 115, d, "saw")
    trem = 0.55 + 0.45 * np.sin(2 * np.pi * (18 + 14 * t / d) * t)
    x = lp(base, 900) * trem
    rasp = bp(noise(d), 300, 1400) * 0.35 * trem
    e = np.clip(t / (d * 0.85), 0, 1) ** 1.5
    e[-int(0.05 * SR):] *= np.linspace(1, 0, int(0.05 * SR))
    return (x + rasp) * e


def sfx_goose_windup(v):
    d = 0.55
    t = t_axis(d)
    honk = lp(sweep(330, 300, 0.16, "saw"), 1800) * env(int(0.16 * SR), 0.01, 0.1)
    hiss = bp(noise(d), 2500, 7500) * np.clip(t / d, 0, 1) ** 1.2
    hiss *= 0.7 + 0.3 * np.sin(2 * np.pi * 11 * t)
    hiss[-int(0.04 * SR):] *= np.linspace(1, 0, int(0.04 * SR))
    return mix(honk * 0.8, hiss * 0.9)


def sfx_mouse_windup(v):
    d = 0.5
    x = np.zeros(int(d * SR))
    for i, (at, f) in enumerate([(0.0, 1500), (0.17, 1750), (0.3, 2050), (0.4, 2400)]):
        sd = 0.08 - i * 0.01
        s = sweep(f, f * 1.35, sd) * env(int(sd * SR), 0.004, 0.04)
        place(x, s * (0.6 + 0.13 * i), at)
    return x


def sfx_boss_hit(v):
    d = 0.3
    t = t_axis(d)
    metal = (np.sin(2 * np.pi * 523 * t) + 0.6 * np.sin(2 * np.pi * 1187 * t) + 0.4 * np.sin(2 * np.pi * 1903 * t))
    metal *= env(len(t), 0.001, 0.12)
    thud = sweep(160, 60, 0.15) * env(int(0.15 * SR), 0.001, 0.06)
    return mix(metal * 0.5, thud)


def sfx_boss_windup(v):
    d = 0.6
    t = t_axis(d)
    charge = sweep(180, 720, d, "saw")
    charge = lp(charge, 2500) * (0.6 + 0.4 * np.sin(2 * np.pi * (8 + 20 * t / d) * t))
    e = np.clip(t / d, 0, 1) ** 1.3
    e[-int(0.04 * SR):] *= np.linspace(1, 0, int(0.04 * SR))
    return charge * e


def sfx_boss_shoot(v):
    d = 0.25
    zap = lp(sweep(1400, 300, d, "square"), 4000) * env(int(d * SR), 0.002, 0.15)
    return zap


def sfx_explosion(v):
    d = 0.9
    t = t_axis(d)
    rumble = lp(noise(d), 900) * env(len(t), 0.003, 0.4)
    crack = hp(noise(0.08), 1500) * env(int(0.08 * SR), 0.001, 0.03)
    boom = sweep(90, 35, 0.5) * env(int(0.5 * SR), 0.002, 0.25)
    return mix(rumble, crack * 0.6, boom * 1.2)


def sfx_ui_click(v):
    d = 0.05
    wood = sweep(1300, 900, d) * env(int(d * SR), 0.0005, 0.012)
    tick = bp(noise(0.01), 2500, 8000) * env(int(0.01 * SR), 0.0002, 0.004)
    return mix(wood, tick * 0.5)


def sfx_level_complete(v):
    x = np.zeros(int(1.6 * SR))
    for i, m in enumerate([62, 65, 69, 74]):
        place(x, dombra(midi(m), 0.6, 0.7), i * 0.11)
    for m in [62, 69, 74, 77]:
        place(x, dombra(midi(m), 1.1, 0.6, 0.6), 0.5)
    return x


def sfx_win(v):
    x = np.zeros(int(3.0 * SR))
    melody = [(62, 0), (65, 0.12), (69, 0.24), (74, 0.36), (72, 0.6), (74, 0.72), (77, 0.84)]
    for m, at in melody:
        place(x, dombra(midi(m), 0.6, 0.8), at)
    for k in range(6):
        for s, m in enumerate([50, 57, 62, 66]):
            place(x, dombra(midi(m), 1.2, 0.6, 0.45), 1.1 + k * 0.09 + s * 0.012)
    place(x, flute(midi(81), 1.4, 0.35), 1.1)
    place(x, drum(0.5, 0.9), 1.1)
    return x


def sfx_lose(v):
    x = np.zeros(int(1.8 * SR))
    for i, m in enumerate([69, 65, 62]):
        place(x, dombra(midi(m), 0.8, 0.4), i * 0.28)
    d = 0.7
    slide = lp(sweep(midi(62), midi(55), d, "tri"), 1500) * env(int(d * SR), 0.02, 0.4) * 0.4
    place(x, slide, 0.84)
    return x


SFX = {
    "jump": (sfx_jump, 2),
    "shoot": (sfx_shoot, 2),
    "coin": (sfx_coin, 2),
    "heal": (sfx_heal, 1),
    "player_hurt": (sfx_player_hurt, 2),
    "enemy_hit": (sfx_enemy_hit, 3),
    "enemy_death": (sfx_enemy_death, 1),
    "fox_windup": (sfx_fox_windup, 1),
    "goose_windup": (sfx_goose_windup, 1),
    "mouse_windup": (sfx_mouse_windup, 1),
    "boss_hit": (sfx_boss_hit, 1),
    "boss_windup": (sfx_boss_windup, 1),
    "boss_shoot": (sfx_boss_shoot, 1),
    "explosion": (sfx_explosion, 1),
    "ui_click": (sfx_ui_click, 1),
    "level_complete": (sfx_level_complete, 1),
    "win": (sfx_win, 1),
    "lose": (sfx_lose, 1),
}


# ---------- music ----------
def loop_render(length, events):
    """Render with a tail, then fold the tail back to the start: a seamless loop."""
    tail = 3.0
    L = np.zeros((int((length + tail) * SR), 2))
    for x, at, pan in events:
        i = int(at * SR)
        j = min(len(L), i + len(x))
        L[i:j, 0] += x[: j - i] * (1 - max(pan, 0))
        L[i:j, 1] += x[: j - i] * (1 + min(pan, 0))
    n = int(length * SR)
    out = L[:n].copy()
    out[: len(L) - n] += L[n:]
    return out


def music_menu():
    bpm = 76
    beat = 60 / bpm
    bars = 8
    length = bars * 4 * beat
    chords = [(50, 57, 62, 65), (48, 55, 60, 64), (46, 53, 58, 62), (45, 52, 57, 61)] * 2
    ev = []
    for b, ch in enumerate(chords):
        t0 = b * 4 * beat
        ev.append((dombra(midi(ch[0] - 12), 4 * beat, 0.3, 0.5), t0, 0))
        arp = [ch[1], ch[2], ch[3], ch[2], ch[1] + 12, ch[2], ch[3], ch[2]]
        for k, m in enumerate(arp):
            ev.append((dombra(midi(m), beat * 1.6, 0.45, 0.35), t0 + k * beat / 2, -0.25))
    mel = [(74, 0, 2), (72, 2, 1), (69, 3, 1), (70, 4, 3), (69, 7, 1),
           (67, 8, 2), (65, 10, 2), (64, 12, 3), (65, 15, 1),
           (74, 16, 2), (77, 18, 2), (76, 20, 2), (74, 22, 2),
           (72, 24, 2), (70, 26, 1), (69, 27, 1), (69, 28, 4)]
    for m, at, ln in mel:
        ev.append((flute(midi(m), ln * beat * 0.95, 0.3), at * beat, 0.3))
    pad_t = t_axis(length)
    # whole number of cycles in the loop, so the drone has no click at the seam
    f1 = round(midi(38) * length) / length
    f2 = round(midi(45) * length) / length
    drone = 0.05 * (np.sin(2 * np.pi * f1 * pad_t) + 0.5 * np.sin(2 * np.pi * f2 * pad_t))
    out = loop_render(length, ev)
    out[:, 0] += drone
    out[:, 1] += drone
    return out


def music_level():
    bpm = 132
    beat = 60 / bpm
    bars = 16
    length = bars * 4 * beat
    prog = [(50, 57, 62), (50, 57, 62), (48, 55, 60), (48, 55, 60),
            (46, 53, 58), (48, 55, 60), (45, 52, 57), (45, 52, 57)] * 2
    gallop = [0, 0.25, 0.5]  # "da-da-DUM" kui strum on every beat
    ev = []
    for b, ch in enumerate(prog):
        t0 = b * 4 * beat
        ev.append((dombra(midi(ch[0] - 12), 2 * beat, 0.35, 0.6), t0, 0))
        ev.append((dombra(midi(ch[0] - 12), 2 * beat, 0.35, 0.5), t0 + 2 * beat, 0))
        for q in range(4):
            for g, off in enumerate(gallop):
                acc = 0.5 if g == 2 else 0.3
                for s, m in enumerate(ch[1:]):
                    ev.append((dombra(midi(m), beat * 0.6, 0.55, acc * 0.6), t0 + (q + off) * beat + s * 0.008, -0.3))
            ev.append((drum(0.3, 0.8 if q % 2 == 0 else 0.5), t0 + q * beat, 0))
            ev.append((shaker(0.25), t0 + (q + 0.5) * beat, 0.4))
    riff = [74, 0, 77, 76, 74, 0, 72, 69, 70, 0, 69, 67, 69, 0, 0, 0,
            74, 0, 77, 79, 81, 0, 79, 77, 76, 0, 74, 72, 74, 0, 0, 0]
    for half in range(2):
        for k, m in enumerate(riff):
            if m:
                ev.append((dombra(midi(m), beat, 0.8, 0.45), (half * 32 + k) * beat, 0.35))
    for k, (m, at, ln) in enumerate([(81, 32, 4), (79, 36, 4), (77, 40, 4), (76, 44, 4),
                                       (74, 48, 6), (76, 54, 2), (77, 56, 4), (76, 60, 4)]):
        ev.append((flute(midi(m), ln * beat * 0.95, 0.22), at * beat, 0.2))
    return loop_render(length, ev)


def write(path, x):
    x = np.clip(x, -1, 1)
    wavfile.write(path, SR, (x * 32767).astype(np.int16))


if __name__ == "__main__":
    os.makedirs(os.path.join(OUT, "SFX"), exist_ok=True)
    os.makedirs(os.path.join(OUT, "Music"), exist_ok=True)
    for name, (fn, count) in SFX.items():
        for v in range(count):
            x = norm(fade_out(fn(v)), 0.8)
            fname = f"{name}_{v + 1}.wav" if count > 1 else f"{name}.wav"
            write(os.path.join(OUT, "SFX", fname), x)
            print("sfx", fname)
    for name, fn in [("music_menu", music_menu), ("music_level", music_level)]:
        x = fn()
        x = np.stack([lp(x[:, 0], 6500), lp(x[:, 1], 6500)], axis=1)  # warmer, less hiss
        x = x * (10 ** (-19 / 20) / np.sqrt((x ** 2).mean()))         # same loudness for both tracks
        x = x * min(1.0, 0.9 / np.abs(x).max())
        write(os.path.join(OUT, "Music", name + ".wav"), x)
        print("music", name, round(len(x) / SR, 1), "s")
