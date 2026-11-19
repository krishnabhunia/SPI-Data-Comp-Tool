"""Every colour theme in UiTheme must stay readable (WCAG AA: text 4.5:1, lines and glyphs 3:1)."""

import re
import unittest
from pathlib import Path

PROJ = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"
THEME = (PROJ / "UiTheme.cs").read_text(encoding="utf-8-sig")
THEMES = ("Ocean", "Berry", "Forest", "Midnight")


def palette(name: str) -> dict:
    block = re.search(rf"public static readonly UiPalette {name} = new UiPalette\s*\{{(.*?)\}};", THEME, re.S)
    if not block:
        raise AssertionError(f"theme {name} not found in UiTheme.cs")
    return {key: tuple(int(rgb[i:i + 2], 16) for i in (0, 2, 4))
            for key, rgb in re.findall(r'(\w+) = Hex\("([0-9a-f]{6})"\)', block.group(1))}


def contrast(a: tuple, b: tuple) -> float:
    def lum(c):
        def ch(v):
            v /= 255
            return v / 12.92 if v <= 0.03928 else ((v + 0.055) / 1.055) ** 2.4
        return 0.2126 * ch(c[0]) + 0.7152 * ch(c[1]) + 0.0722 * ch(c[2])
    hi, lo = sorted((lum(a), lum(b)), reverse=True)
    return (hi + 0.05) / (lo + 0.05)


class ThemeContrast(unittest.TestCase):
    TEXT = [  # (text, background) as the windows draw them
        ("TextColor", "WindowBack"), ("TextColor", "CardBack"), ("TextColor", "FieldBack"),
        ("MutedText", "WindowBack"), ("MutedText", "CardBack"),
        ("HeadText", "Head"),                                                    # header band, grid headers
        ("AccentText", "Accent"), ("AccentText", "AccentHover"), ("AccentText", "AccentPressed"),   # main button, selected rows
        ("DisabledText", "DisabledBack"),
        ("TextColor", "HoverBack"), ("TextColor", "PressedBack"),               # buttons, menu items, tiles
        ("MutedText", "HoverBack"), ("MutedText", "PressedBack"),               # the line under a tile's name
        ("TextColor", "SourceTint"), ("TextColor", "TargetTint"),               # Source / Target captions and grid columns
        ("StatusText", "StatusBack"),
        ("Danger", "CardBack"), ("Danger", "WindowBack"), ("Danger", "FieldBack"), ("Danger", "StatusBack"),
        ("Success", "CardBack"), ("Success", "WindowBack"), ("Success", "FieldBack"), ("Success", "StatusBack"),
        ("Info", "CardBack"), ("Info", "WindowBack"), ("Info", "FieldBack"), ("Info", "StatusBack"),
    ]
    SHAPES = [  # lines, strips and glyphs: 3:1 against what they sit on
        ("Source", "SourceTint"), ("Target", "TargetTint"), ("Source", "CardBack"), ("Target", "CardBack"),
        ("Tile1", "CardBack"), ("Tile2", "CardBack"), ("Tile3", "CardBack"), ("Tile4", "CardBack"),
        ("Tile1", "HoverBack"), ("Tile2", "HoverBack"), ("Tile3", "HoverBack"), ("Tile4", "HoverBack"),
        ("FieldBorder", "FieldBack"), ("FieldBorder", "CardBack"), ("Accent", "CardBack"), ("Accent", "Border"),
    ]

    def test_every_theme_sets_every_colour(self) -> None:
        declared = re.search(r"internal sealed class UiPalette\s*\{(.*?)public Color Tile\(", THEME, re.S).group(1)
        fields = set()
        for line in re.findall(r"public Color ([^;]+);", declared):
            fields |= {name.strip() for name in line.split(",")}
        self.assertGreaterEqual(len(fields), 28)
        for theme in THEMES:
            with self.subTest(theme=theme):
                self.assertEqual(set(palette(theme)), fields)

    def test_text_is_readable_in_every_theme(self) -> None:
        for theme in THEMES:
            colours = palette(theme)
            for text, back in self.TEXT:
                with self.subTest(theme=theme, text=text, back=back):
                    self.assertGreaterEqual(contrast(colours[text], colours[back]), 4.5)

    def test_lines_and_glyphs_are_visible_in_every_theme(self) -> None:
        for theme in THEMES:
            colours = palette(theme)
            for shape, back in self.SHAPES:
                with self.subTest(theme=theme, shape=shape, back=back):
                    self.assertGreaterEqual(contrast(colours[shape], colours[back]), 3.0)

    def test_source_and_target_are_told_apart(self) -> None:
        for theme in THEMES:
            colours = palette(theme)
            with self.subTest(theme=theme):
                self.assertNotEqual(colours["Source"], colours["Target"])
                self.assertNotEqual(colours["SourceTint"], colours["TargetTint"])

    def test_cells_marked_yellow_keep_black_text(self) -> None:
        self.assertGreaterEqual(contrast((0, 0, 0), (255, 255, 0)), 4.5)
        self.assertIn("e.CellStyle.BackColor.ToArgb() != Color.Yellow.ToArgb()", THEME)
        self.assertIn("e.CellStyle.ForeColor = Color.Black;", THEME)

    def test_status_colours_do_not_collide_between_themes(self) -> None:
        # Readable() recognises a colour taken from any theme by its value, so one value must mean one thing
        meaning = {}
        for theme in THEMES:
            colours = palette(theme)
            for role in ("TextColor", "MutedText", "Danger", "Success", "Info"):
                self.assertIn(meaning.setdefault(colours[role], role), (role,), f"{theme}.{role}")

    def test_ocean_is_the_default_and_only_midnight_is_dark(self) -> None:
        self.assertIn("public static readonly UiPalette[] All = { Ocean, Berry, Forest, Midnight };", THEME)
        self.assertEqual(re.findall(r'Name = "(\w+)", Title = "[^"]+", Dark = true', THEME), ["Midnight"])
        self.assertIn("return All[0];", THEME)

    def test_disabled_buttons_paint_their_own_readable_text(self) -> None:
        # Windows draws disabled text light grey whatever ForeColor is; the theme must paint it itself,
        # and the main button must not keep its accent colour when disabled
        self.assertIn("b.Paint += PaintDisabled;", THEME)
        self.assertIn("b.EnabledChanged += (s, e) => ColorMainButton((Button)s);", THEME)
        self.assertIn("b.BackColor = b.Enabled ? p.Accent : p.DisabledBack;", THEME)


if __name__ == "__main__":
    unittest.main()
