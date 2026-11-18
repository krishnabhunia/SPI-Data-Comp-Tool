"""Button colours in UiTheme must stay readable (WCAG AA: contrast 4.5:1 or more)."""

import re
import unittest
from pathlib import Path

THEME = (Path(__file__).resolve().parents[1] / "SPI Data Comp Tool" / "UiTheme.cs").read_text(encoding="utf-8-sig")


def colour(name: str) -> tuple:
    if name == "White" or re.search(rf"\b{name} = Color\.White;", THEME):
        return (255, 255, 255)
    m = re.search(rf"\b{name} = Color\.FromArgb\((\d+), (\d+), (\d+)\)", THEME)
    if not m:
        raise AssertionError(f"colour {name} not found in UiTheme.cs")
    return tuple(int(x) for x in m.groups())


def contrast(a: tuple, b: tuple) -> float:
    def lum(c):
        def ch(v):
            v /= 255
            return v / 12.92 if v <= 0.03928 else ((v + 0.055) / 1.055) ** 2.4
        return 0.2126 * ch(c[0]) + 0.7152 * ch(c[1]) + 0.0722 * ch(c[2])
    hi, lo = sorted((lum(a), lum(b)), reverse=True)
    return (hi + 0.05) / (lo + 0.05)


class ButtonContrast(unittest.TestCase):
    PAIRS = [  # (text, background) as the buttons draw them
        ("White", "Accent"), ("White", "AccentHover"), ("White", "AccentPressed"),
        ("DisabledText", "DisabledBack"), ("TextColor", "CardBack"),
    ]

    def test_text_is_readable_on_every_button_state(self) -> None:
        for text, back in self.PAIRS:
            with self.subTest(text=text, back=back):
                self.assertGreaterEqual(contrast(colour(text), colour(back)), 4.5)

    def test_disabled_buttons_paint_their_own_readable_text(self) -> None:
        # Windows draws disabled text light grey whatever ForeColor is; the theme must paint it itself,
        # and the blue main button must not stay blue when disabled
        self.assertIn("b.Paint += PaintDisabled;", THEME)
        self.assertIn("b.EnabledChanged += (s, e) => ColorMainButton((Button)s);", THEME)
        self.assertIn("b.BackColor = b.Enabled ? Accent : DisabledBack;", THEME)


if __name__ == "__main__":
    unittest.main()
