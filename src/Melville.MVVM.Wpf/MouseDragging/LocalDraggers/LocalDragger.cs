﻿using System;
using System.ComponentModel.Design.Serialization;
using System.Windows;
using System.Windows.Forms.Design.Behavior;
using Melville.MVVM.Undo;
using Melville.MVVM.Wpf.EventBindings;

namespace Melville.MVVM.Wpf.MouseDragging.LocalDraggers
{
    public interface ILocalDragger<T> where T: struct
    {
        void NewPoint(MouseMessageType type, T point);
    }

    public static class LocalDragger
    {
        public static ILocalDragger<Point> Action(Action<MouseMessageType, Point> action) =>
            new LambdaDragger<Point>(action);
        public static ILocalDragger<Point> Action(Action<Point> action) =>
            Action((_, point)=>action(point));
        public static ILocalDragger<CircularPoint> CircleAction(Action<MouseMessageType, CircularPoint> action) =>
            new LambdaDragger<CircularPoint>(action);
        public static ILocalDragger<CircularPoint> CircleAction(Action<CircularPoint> action) =>
            CircleAction((_, point)=>action(point));

        public static ILocalDragger<T> Undo<T>(UndoEngine undo, ILocalDragger<T> effector)
            where T: struct =>
            new UndoDragger<T>(undo, effector);

        public static ILocalDragger<Point> MinimumDrag(double radius, ILocalDragger<Point> effector) =>
            new MinimumDragger(radius, effector);

        public static ILocalDragger<Point> RectToCircle(
            Point origin, ILocalDragger<CircularPoint> target) =>
            RectToCircle(origin, new Vector(1, 0), target);
        public static ILocalDragger<Point> RectToCircle(
            Point origin, Vector zero, ILocalDragger<CircularPoint> target) =>
            Action((type, pt) => target.NewPoint(type, CircularPoint.FromVectors(zero, pt - origin)));

        public static ILocalDragger<CircularPoint> SnapToAngle(
            int snapPoints, double width, ILocalDragger<CircularPoint> target) =>
            new CircleSnapper(snapPoints, width, target);

        public static ILocalDragger<CircularPoint> SnapMouseUpToAngle(
            int snapPoints, double width, ILocalDragger<CircularPoint> target) =>
            new MouseUpCircleSnapper(snapPoints, width, target);

        public static ILocalDragger<Point> Delta(ILocalDragger<Point> target) =>
            new DeltaDragger(target);

        public static ILocalDragger<Point> InitialPoint(
            double iX, double iY, ILocalDragger<Point> target) => InitialPoint(new Point(iX, iY), target);

        public static ILocalDragger<Point> InitialPoint(Point origin, ILocalDragger<Point> target) =>
            new InitialPointDragger(origin, target);

        public static ILocalDragger<Point> GridSnapping(double snapSize, ILocalDragger<Point> target) =>
            Action((type, point) => target.NewPoint(type,
                new Point(SnapToGrid(snapSize, point.X), SnapToGrid(snapSize, point.Y))));

        private static double SnapToGrid(double snapSize, double dimension) => 
            snapSize * Math.Round(dimension / snapSize);


        //next we need to build a grid snapping dragger and then compoose the field dragger from
        // the RestrictToAxis, InitialPointDragger, new dragger, and the KeySwitching dragger

    }
}