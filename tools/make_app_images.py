"""Draw the app's own icon and pictures (original artwork, no third-party logos).

Run from the repository root:  python tools/make_app_images.py
Writes:
  SPI Data Comp Tool/app.ico                     - window / exe icon (16-256 px)
  SPI Data Comp Tool/Resources/about_side.png    - About box side picture
and puts app.ico into every form's embedded window icon ($this.Icon in *.resx).
Needs Pillow and the Inter font (falls back to DejaVu Sans).
"""
import base64
import re
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"
NAVY, BLUE, SKY = (30, 58, 138), (37, 99, 235), (191, 219, 254)
MINT, MINT_DARK, WHITE = (167, 243, 208), (16, 185, 129), (255, 255, 255)


def font(size, bold=True):
    names = (["Inter-Bold.otf", "DejaVuSans-Bold.ttf"] if bold
             else ["Inter-Regular.otf", "DejaVuSans.ttf"])
    for folder in ["/usr/share/fonts/opentype/inter", "/usr/share/fonts/truetype/dejavu",
                   "C:/Windows/Fonts"]:
        for n in names:
            p = Path(folder) / n
            if p.exists():
                return ImageFont.truetype(str(p), size)
    return ImageFont.load_default()


def gradient(size, top, bottom):
    w, h = size
    img = Image.new("RGB", size)
    d = ImageDraw.Draw(img)
    for y in range(h):
        t = y / max(h - 1, 1)
        d.line([(0, y), (w, y)], fill=tuple(int(a + (b - a) * t) for a, b in zip(top, bottom)))
    return img


def cylinder(d, cx, top, w, h, body, band, outline, stroke):
    """Database cylinder: body, top ellipse, two disk bands."""
    rx, ry = w / 2, w * 0.17
    x0, x1 = cx - rx, cx + rx
    d.rectangle([x0, top + ry, x1, top + h - ry], fill=body)
    d.ellipse([x0, top + h - 2 * ry, x1, top + h], fill=body, outline=outline, width=stroke)
    d.rectangle([x0 + stroke, top + ry, x1 - stroke, top + h - ry], fill=body)
    d.line([(x0, top + ry), (x0, top + h - ry)], fill=outline, width=stroke)
    d.line([(x1, top + ry), (x1, top + h - ry)], fill=outline, width=stroke)
    for k in (1, 2):
        y = top + ry + (h - 2 * ry) * k / 3
        d.arc([x0, y - ry, x1, y + ry], 0, 180, fill=band, width=stroke)
    d.ellipse([x0, top, x1, top + 2 * ry], fill=body, outline=outline, width=stroke)


def arrow(d, x0, x1, y, color, width):
    head = width * 1.9
    d.line([(x0 + head * 0.8, y), (x1 - head * 0.8, y)], fill=color, width=width)
    d.polygon([(x0, y), (x0 + head, y - head * 0.75), (x0 + head, y + head * 0.75)], fill=color)
    d.polygon([(x1, y), (x1 - head, y - head * 0.75), (x1 - head, y + head * 0.75)], fill=color)


def icon_master(n=1024):
    tile = gradient((n, n), BLUE, NAVY)
    mask = Image.new("L", (n, n), 0)
    ImageDraw.Draw(mask).rounded_rectangle([0, 0, n - 1, n - 1], radius=int(n * 0.21), fill=255)
    img = Image.new("RGBA", (n, n), (0, 0, 0, 0))
    img.paste(tile, (0, 0), mask)
    d = ImageDraw.Draw(img)
    s = n / 1024
    cylinder(d, 330 * s, 170 * s, 300 * s, 420 * s, WHITE, SKY, WHITE, int(18 * s))
    cylinder(d, 694 * s, 170 * s, 300 * s, 420 * s, MINT, MINT_DARK, MINT, int(18 * s))
    arrow(d, 190 * s, 834 * s, 780 * s, WHITE, int(56 * s))
    return img


def write_icon(master):
    sizes = [(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)]
    master.save(ROOT / "app.ico", format="ICO", sizes=sizes)


def about_side(master, w=262, h=518):
    img = gradient((w, h), BLUE, NAVY).convert("RGBA")
    ic = master.resize((190, 190), Image.LANCZOS)
    img.paste(ic, ((w - 190) // 2, 60), ic)
    d = ImageDraw.Draw(img)
    for i, line in enumerate(["SPI Data", "Comparison", "Tool"]):
        f = font(34)
        d.text(((w - d.textlength(line, font=f)) / 2, 290 + i * 46), line, font=f, fill=WHITE)
    img.convert("RGB").save(ROOT / "Resources" / "about_side.png", optimize=True)


def update_form_icons():
    """Replace each form's embedded $this.Icon with app.ico."""
    data = base64.b64encode((ROOT / "app.ico").read_bytes()).decode()
    body = "\n".join("        " + data[i:i + 80] for i in range(0, len(data), 80))
    pat = re.compile(r'(<data name="\$this\.Icon" type="System\.Drawing\.Icon, System\.Drawing" '
                     r'mimetype="application/x-microsoft\.net\.object\.bytearray\.base64">\s*<value>)'
                     r'(.*?)(</value>)', re.S)
    count = 0
    for resx in sorted(ROOT.rglob("*.resx")):
        text = resx.read_text(encoding="utf-8-sig")
        new, n = pat.subn(lambda m: m.group(1) + "\n" + body + "\n" + m.group(3), text)
        if n:
            resx.write_text(new, encoding="utf-8-sig")
            count += n
    return count


if __name__ == "__main__":
    m = icon_master()
    write_icon(m)
    about_side(m)
    print("images written to", ROOT)
    print("form icons updated:", update_form_icons())
