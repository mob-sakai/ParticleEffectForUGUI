using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.NanoMonitor{
    public class FixedFont
    {
        private readonly UIVertex[] _tmpVerts = new UIVertex[4];
        private static readonly Dictionary<Font, FixedFont> _fonts = new Dictionary<Font, FixedFont>();
        private static readonly TextGenerator _textGenerator = new TextGenerator(100);

        private static TextGenerationSettings _settings = new TextGenerationSettings
        {
            scaleFactor = 1,
            horizontalOverflow = HorizontalWrapMode.Overflow,
            verticalOverflow = VerticalWrapMode.Overflow,
            alignByGeometry = true,
            textAnchor = TextAnchor.MiddleCenter,
            color = Color.white
        };

        private int _resolution = 0;
        private readonly Font _font;
        private UIVertex[] _verts;
        private UICharInfo[] _charInfos;

        private FixedFont(Font font)
        {
            _font = font;
        }

        public int fontSize => _font.dynamic ? 32 : _font.fontSize;

        public static FixedFont GetOrCreate(Font font)
        {
            if (font == null) return null;
            if (_fonts.TryGetValue(font, out var data)) return data;

            data = new FixedFont(font);
            _fonts.Add(font, data);
            return data;
        }

        public void Invalidate()
        {
            _resolution = 0;
        }

        public void UpdateFont()
        {
            if (!_font) return;

            var mat = _font.material;
            if (!mat) return;

            var tex = mat.mainTexture;
            if (!tex) return;

            var currentResolution = tex.width * tex.height;

            if (_resolution == currentResolution) return;
            _resolution = currentResolution;

            _settings.font = _font;
            _textGenerator.Invalidate();
            _textGenerator.Populate("_!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~", _settings);

            _verts = _textGenerator.GetVerticesArray();
            _charInfos = _textGenerator.GetCharactersArray();

            float offsetX = 0;
            for (var i = 0; i < _verts.Length; i++)
            {
                if ((i & 3) == 0)
                    offsetX = _verts[i].position.x;

                var v = _verts[i];
                v.position -= new Vector3(offsetX, 0);
                _verts[i] = v;
            }
        }

        public float Layout(char c, float offset, float scale)
        {
            if (_charInfos == null) return offset;
            if (c < 0x20 || 0x7e < c) return offset;
            var ci = c - 0x20;

            return offset + _charInfos[ci].charWidth * scale;
        }

        public float Append(VertexHelper toFill, char c, float offset, float scale, Color color)
        {
            if (_verts == null || _charInfos == null) return offset;
            if (c < 0x20 || 0x7e < c) return offset;
            var ci = c - 0x20;

            for (var i = 0; i < 4; i++)
            {
                _tmpVerts[i] = _verts[ci * 4 + i];
                _tmpVerts[i].position = _tmpVerts[i].position * scale + new Vector3(offset, 0);
                _tmpVerts[i].color = ci == 0 ? Color.clear : color;
            }

            toFill.AddUIVertexQuad(_tmpVerts);
            return offset + _charInfos[ci].charWidth * scale;
        }

        public void Fill(VertexHelper toFill, Color color, RectTransform tr)
        {
            if (_verts == null || _charInfos == null) return;
            const int ci = '*' - 0x20;
            var uv = (_verts[ci * 4].uv0 + _verts[ci * 4 + 2].uv0) / 2;

            var rect = tr.rect;
            var size = rect.size / 2;
            var offset = (new Vector2(0.5f, 0.5f) - tr.pivot) * rect.size;

            for (var i = 0; i < 4; i++)
            {
                _tmpVerts[i] = new UIVertex();
                _tmpVerts[i].uv0 = uv;
                _tmpVerts[i].color = color;

                if (i == 0)
                    _tmpVerts[i].position = new Vector2(-size.x, -size.y) + offset;
                else if (i == 1)
                    _tmpVerts[i].position = new Vector2(-size.x, size.y) + offset;
                else if (i == 2)
                    _tmpVerts[i].position = new Vector2(size.x, size.y) + offset;
                else
                    _tmpVerts[i].position = new Vector2(size.x, -size.y) + offset;
            }

            toFill.AddUIVertexQuad(_tmpVerts);
        }
    }
}
