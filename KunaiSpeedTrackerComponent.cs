using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace LiveSplit.KunaiSpeedTracker
{
    public class KunaiSpeedTrackerComponent : IComponent
    {
        private readonly KunaiMemory _mem;
        private readonly LiveSplitState _state;

        private readonly SimpleLabel _xNameLabel = new SimpleLabel();
        private readonly SimpleLabel _xValLabel = new SimpleLabel();

        private readonly SimpleLabel _yNameLabel = new SimpleLabel();
        private readonly SimpleLabel _yValLabel = new SimpleLabel();

        private readonly SimpleLabel _totalNameLabel = new SimpleLabel();
        private readonly SimpleLabel _totalValLabel = new SimpleLabel();

        private readonly Stopwatch _watch = Stopwatch.StartNew();
        private readonly List<(PointF, long)> _speedData = new List<(PointF, long)>();

        public string ComponentName { get; }

        public KunaiSpeedTrackerComponent(LiveSplitState state, string name)
        {
            ComponentName = name;
            _state = state;

            _mem = new KunaiMemory();
            Cache = new GraphicsCache();
        }

        public void Dispose()
        {
            _mem.Dispose();
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            try
            {
                _mem.Hook();
            }
            catch
            {
                // ignored
            }

            if (!_mem.IsHooked)
            {
                return;
            }

            _xNameLabel.Text = "X:";
            _yNameLabel.Text = "Y:";
            _totalNameLabel.Text = "Total:";

            PointF speed = default;
            try
            {
                speed = _mem.GetPlayerSpeed();
            }
            catch
            {
                // ignored
            }

            // Calculate average
            long time = _watch.ElapsedMilliseconds;
            float totalX = 0f;
            float totalY = 0f;

            _speedData.Add((speed, time));
            for (int i = _speedData.Count - 1; i >= 0; i--)
            {
                if (time - _speedData[i].Item2 >= 1000L)
                {
                    _speedData.RemoveAt(i);
                    continue;
                }

                totalX += _speedData[i].Item1.X;
                totalY += _speedData[i].Item1.Y;
            }

            float avgX = totalX / _speedData.Count;
            float avgY = totalY / _speedData.Count;
            float magnitude = (float)Math.Sqrt(avgX * avgX + avgY * avgY);

            _xValLabel.Text = avgX.ToString("0.00");
            _yValLabel.Text = avgY.ToString("0.00");
            _totalValLabel.Text = magnitude.ToString("0.00");

            Cache.Restart();
            Cache[nameof(_xNameLabel)] = _xNameLabel.Text;
            Cache[nameof(_xValLabel)] = _xValLabel.Text;
            Cache[nameof(_yNameLabel)] = _yNameLabel.Text;
            Cache[nameof(_yValLabel)] = _yValLabel.Text;
            Cache[nameof(_totalNameLabel)] = _totalNameLabel.Text;
            Cache[nameof(_totalValLabel)] = _totalValLabel.Text;

            if (invalidator != null && Cache.HasChanged)
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            Font font = state.LayoutSettings.TextFont;

            // Calculate text height
            float textHeight = g.MeasureString("A", font).Height;
            VerticalHeight = 3.6f * textHeight;

            // Padding
            PaddingTop = Math.Max(0, (VerticalHeight - 0.75f * textHeight) / 2f);
            PaddingBottom = PaddingTop;

            // Width
            float textWidth = g.MeasureString("0.00", font).Width;
            HorizontalWidth = _xNameLabel.X + _xNameLabel.ActualWidth +
                              (textWidth > _xValLabel.ActualWidth ? textWidth : _xValLabel.ActualWidth) + 5;

            // Set x name label
            _xNameLabel.HorizontalAlignment = StringAlignment.Near;
            _xNameLabel.VerticalAlignment = StringAlignment.Center;
            _xNameLabel.X = 5;
            _xNameLabel.Y = 0;
            _xNameLabel.Width = width - textWidth - 5;
            _xNameLabel.Height = VerticalHeight / 4f;
            _xNameLabel.Font = font;
            _xNameLabel.Brush = new SolidBrush(state.LayoutSettings.TextColor);
            _xNameLabel.HasShadow = state.LayoutSettings.DropShadows;
            _xNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            _xNameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            _xNameLabel.Draw(g);

            // Set x val label
            _xValLabel.HorizontalAlignment = StringAlignment.Far;
            _xValLabel.VerticalAlignment = StringAlignment.Center;
            _xValLabel.X = 5;
            _xValLabel.Y = 0;
            _xValLabel.Width = width - 10;
            _xValLabel.Height = VerticalHeight / 4f;
            _xValLabel.Font = font;
            _xValLabel.Brush = new SolidBrush(state.LayoutSettings.TextColor);
            _xValLabel.HasShadow = state.LayoutSettings.DropShadows;
            _xValLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            _xValLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            _xValLabel.Draw(g);

            // Set y name label
            _yNameLabel.HorizontalAlignment = StringAlignment.Near;
            _yNameLabel.VerticalAlignment = StringAlignment.Center;
            _yNameLabel.X = 5;
            _yNameLabel.Y = VerticalHeight / 3f;
            _yNameLabel.Width = width - textWidth - 5;
            _yNameLabel.Height = VerticalHeight / 4f;
            _yNameLabel.Font = font;
            _yNameLabel.Brush = new SolidBrush(state.LayoutSettings.TextColor);
            _yNameLabel.HasShadow = state.LayoutSettings.DropShadows;
            _yNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            _yNameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            _yNameLabel.Draw(g);

            // Set y val label
            _yValLabel.HorizontalAlignment = StringAlignment.Far;
            _yValLabel.VerticalAlignment = StringAlignment.Center;
            _yValLabel.X = 5;
            _yValLabel.Y = VerticalHeight / 3f;
            _yValLabel.Width = width - 10;
            _yValLabel.Height = VerticalHeight / 4f;
            _yValLabel.Font = font;
            _yValLabel.Brush = new SolidBrush(state.LayoutSettings.TextColor);
            _yValLabel.HasShadow = state.LayoutSettings.DropShadows;
            _yValLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            _yValLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            _yValLabel.Draw(g);

            // Set total name label
            _totalNameLabel.HorizontalAlignment = StringAlignment.Near;
            _totalNameLabel.VerticalAlignment = StringAlignment.Center;
            _totalNameLabel.X = 5;
            _totalNameLabel.Y = VerticalHeight * 2 / 3f;
            _totalNameLabel.Width = width - textWidth - 5;
            _totalNameLabel.Height = VerticalHeight / 4f;
            _totalNameLabel.Font = font;
            _totalNameLabel.Brush = new SolidBrush(state.LayoutSettings.TextColor);
            _totalNameLabel.HasShadow = state.LayoutSettings.DropShadows;
            _totalNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            _totalNameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            _totalNameLabel.Draw(g);

            // Set total val label
            _totalValLabel.HorizontalAlignment = StringAlignment.Far;
            _totalValLabel.VerticalAlignment = StringAlignment.Center;
            _totalValLabel.X = 5;
            _totalValLabel.Y = VerticalHeight * 2 / 3f;
            _totalValLabel.Width = width - 10;
            _totalValLabel.Height = VerticalHeight / 4f;
            _totalValLabel.Font = font;
            _totalValLabel.Brush = new SolidBrush(state.LayoutSettings.TextColor);
            _totalValLabel.HasShadow = state.LayoutSettings.DropShadows;
            _totalValLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            _totalValLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            _totalValLabel.Draw(g);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            return document.CreateElement("Settings");
        }

        public void SetSettings(XmlNode settings)
        {
        }

        public System.Windows.Forms.Control GetSettingsControl(LayoutMode mode)
        {
            return null;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
        }

        public float HorizontalWidth { get; private set; }

        public float MinimumHeight { get; private set; }

        public float VerticalHeight { get; private set; }

        public float MinimumWidth => _totalNameLabel.X + _totalValLabel.ActualWidth;

        public float PaddingTop { get; private set; }

        public float PaddingBottom { get; private set; }

        public float PaddingLeft => 7f;

        public float PaddingRight => 7f;

        public IDictionary<string, Action> ContextMenuControls => null;

        public GraphicsCache Cache { get; set; }
    }
}
