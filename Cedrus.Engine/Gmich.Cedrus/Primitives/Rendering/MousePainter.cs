﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Gmich.Cedrus.Primitives
{
    public class MousePainter : IPainter
    {
        private ShapeDeclaration shapeDeclaration;
        private Action<MouseState> trackShape;
        private readonly GraphicsDevice device;
        private readonly DynamicShape processedShape;
        private readonly int viewportWidth;
        private readonly int viewportHeight;
        public EventHandler ShapeAdded;

        public List<IShape> Shapes
        {
            get;
        } = new List<IShape>();

        public MousePainter(GraphicsDevice graphicsDevice, int viewportWidth, int viewportHeight)
        {
            device = graphicsDevice;
            trackShape = InitializeShape;
            this.viewportWidth= viewportWidth;
            this.viewportHeight = viewportHeight;
            processedShape = new DynamicShape(Color.White,device, viewportWidth, viewportHeight);
        }

        public void Paint(double timeDelta)
        {
            var mouseState = Mouse.GetState();

            if (device.Viewport.Bounds.Contains(mouseState.Position))
            {
                trackShape(mouseState);
            }
        }

        public void DrawActiveShape()
        {
            processedShape.Draw();
        }

        private void CaptureMousePosition(MouseState state)
        {
            if (state.LeftButton == ButtonState.Pressed)
            {
                var mousePosition = new Vector2(state.X, state.Y);
                if (shapeDeclaration.Capture(mousePosition))
                {
                    processedShape.AddVertex(mousePosition);
                }
            }
            else
            {
                trackShape = AddShape;
            }
        }

        private void InitializeShape(MouseState state)
        {
            if (state.LeftButton == ButtonState.Pressed)
            {
                shapeDeclaration = new ShapeDeclaration(new Vector2(state.X, state.Y));
                processedShape.AddVertex(shapeDeclaration.ViewportOffset);
                processedShape.AddVertex(shapeDeclaration.ViewportOffset);
                trackShape = CaptureMousePosition;
            }
        }

        private void AddShape(MouseState state)
        {
            int vertices = shapeDeclaration.VerticesPosition.Count;

            if (vertices == 1)
            {
                shapeDeclaration.VerticesPosition.Add(shapeDeclaration.VerticesPosition[0] + Vector2.One);
            }
            if (vertices != 0)
            {
                Shapes.Add(
                 new FixedBoundsShape(
                     shapeDeclaration.VerticesPosition,
                     shapeDeclaration.ViewportOffset.X,
                     shapeDeclaration.ViewportOffset.Y,
                     viewportWidth,
                     viewportHeight,
                     Color.White,
                     device));
                ShapeAdded?.Invoke(this, EventArgs.Empty);
            }
            trackShape = InitializeShape;
            processedShape.Reset();
        }

    }
}
