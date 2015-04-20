using System;
using Petrrollus.EnemyInfoStorage;
using Robocode;
using Petrrollus.Helpers;
using System.Console;
using Robocode.Util;

namespace Petrrollus
{
    internal interface IMovementProvider
    {
        void Init(Petrrollus robot, EnemyInfo info);
        void FirstTurn();
        void EveryTurn();
    }

    class Chassis : IMovementProvider
    {
        Petrrollus robot;
        EnemyInfo info;

        double width;
        double height;

        int dir;

        const double avoidWallProjectionLenght = 100;

        const double initFollowingProjectionLenght = 150;
        const double initFollowingProjectionAngle = Math.PI / 4;

        const double projectionAngleTest = 0.1672664626; //circa 10 degrees
        const double followingTreshold = 300;
        const double maxNormalTurnMultiplyer = 0.55;

        const int predictionFollowTicks = 25;

        private Status status;

        public void Init(Petrrollus robot, EnemyInfo info)
        {
            this.robot = robot;
            this.info = info;

            status = Status.Circling;
        }

        public void FirstTurn()
        {
            width = robot.BattleFieldWidth;
            height = robot.BattleFieldHeight;

            dir = 1;

            robot.SetAhead(100);
        }

        public void EveryTurn()
        {
            double behindWall = castProjectionBehindWalls(avoidWallProjectionLenght, 0);
            updateStatus(behindWall);

            if (status == Status.AvoidingWall)
            {
                avoidWall(behindWall);
            }
            else if (status == Status.Following)
            {
                followEnemyRobot();

            }
            else if (status == Status.Circling)
            {
                circle();
            }

            robot.SetAhead(100 * dir);
        }

        private void updateStatus(double behindWall)
        {
            if (behindWall > 0) { status = Status.AvoidingWall; }
            else if (canStartFollowing()) { status = Status.Following; }
            else { status = Status.Circling; }
        }

        private void circle()
        {
            robot.MaxVelocity = Rules.MAX_VELOCITY;

            double turnAngle = (Rules.GetTurnRateRadians(robot.Velocity) * maxNormalTurnMultiplyer);
            turnAngle = (info.Bearings.GetCurrent() > 0) ? turnAngle : -turnAngle;
            robot.SetTurnRightRadians(turnAngle);
        }

        private void followEnemyRobot()
        {
            robot.MaxVelocity = Rules.MAX_VELOCITY;

            Coordinates futurePos = info.GetGuessPositionEducated(predictionFollowTicks);
            double positionAngle = MiscHelper.GetAngleBetweenPositions(robot.Position, futurePos);
            double turnAngle = Utils.NormalRelativeAngle(positionAngle - Utils.NormalRelativeAngle(robot.HeadingRadians));

            robot.SetTurnRightRadians(turnAngle);
        }

        private void avoidWall(double behindWall)
        {
            double urgency = behindWall / avoidWallProjectionLenght;
            robot.MaxVelocity = urgency * Rules.MAX_VELOCITY;

            Sides changeHeadingTo = decideHowToAvoidWall();
            double turnAngle = (changeHeadingTo == Sides.Right) ? double.PositiveInfinity : double.NegativeInfinity;
            robot.SetTurnRightRadians(double.PositiveInfinity);
        }

        private bool canStartFollowing()
        {
            return castProjectionBehindWalls(initFollowingProjectionLenght, 0) == 0 &&
                            castProjectionBehindWalls(initFollowingProjectionLenght, initFollowingProjectionAngle) == 0 &&
                            castProjectionBehindWalls(initFollowingProjectionAngle, -initFollowingProjectionAngle) == 0;
        }

        private Sides decideHowToAvoidWall()
        {
            double positiveDiff = castProjectionBehindWalls(avoidWallProjectionLenght, projectionAngleTest);
            double negativeDiff = castProjectionBehindWalls(avoidWallProjectionLenght, - projectionAngleTest);

            return (positiveDiff > negativeDiff) ? Sides.Right : Sides.Left;
        }

        private double castProjectionBehindWalls(double projectionLenght , double angleDiff)
        {
            double angle = robot.HeadingRadians + angleDiff;

            double incX = projectionLenght * Math.Sin(angle);
            double incY = projectionLenght * Math.Cos(angle);

            Coordinates projectedPosition = robot.Position + new Coordinates(incX, incY);

            double xBehind = 0;
            if (projectedPosition.X > width) { xBehind = projectedPosition.X - width; }
            if (projectedPosition.X < 0) { xBehind = Math.Abs(projectedPosition.X); }

            double yBehind = 0;
            if (projectedPosition.Y > height) { yBehind = projectedPosition.Y - height; }
            if (projectedPosition.Y < 0) { yBehind = Math.Abs(projectedPosition.Y); }

            return yBehind + xBehind;
        }
    }

    enum Status { AvoidingWall, Circling, Following}
    enum Sides { Left, Right }
}
