using System.Console;
using Petrrollus.Helpers;
using Petrrollus.Radars;
using Petrrollus.EnemyInfoStorage;
using Petrrollus.Guns;
using Robocode.Util;
using Robocode;
using System;

namespace Petrrollus
{
    public class Petrrollus : AdvancedRobot
    {
        public double RadarHeadingDiffRadians { get { return Utils.NormalRelativeAngle(RadarHeadingRadians - HeadingRadians); } }
        public double GunHeadingDiffRadians { get { return Utils.NormalRelativeAngle(GunHeadingRadians - HeadingRadians); } }

        public Coordinates Position { get { return new Coordinates(this.X, this.Y); } }

        private EnemyInfo enemyInfo;

        private IRadarProvider radarProvider;
        private IAimAndShootProvider gun;
        private IMovementProvider wheels;

        public Petrrollus()
        {
            enemyInfo = new EnemyInfo();

            radarProvider = new NarrowLockRadar();
            radarProvider.Init(this, enemyInfo);

            gun = new PrecisionGun();
            gun.Init(this, enemyInfo);

            wheels = new Chassis();
            wheels.Init(this, enemyInfo);
        }

        public override void Run()
        {
            this.SetColors(System.Drawing.Color.Black, System.Drawing.Color.BlueViolet, System.Drawing.Color.Black);

            radarProvider.FirstTurn();
            gun.FirstTurn();
            wheels.FirstTurn();

            while (true)
            {
                radarProvider.EveryTurn();
                gun.EveryTurn();
                wheels.EveryTurn();

                Execute();
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            enemyInfo.UpdateInfo(evnt, this.Position, this.HeadingRadians);
            radarProvider.OnScan(evnt);
        }

        public override void OnSkippedTurn(SkippedTurnEvent evnt)
        {
            WriteLine("Skipped turn " + evnt.SkippedTurn + " " + evnt.Time);
        }

        public override void OnStatus(StatusEvent e)
        {

        }

        public override void OnPaint(IGraphics graphics)
        {
            DebugDrawer.DebugDrawRect(enemyInfo.GetGuessPositionHistory(10), 10, System.Drawing.Brushes.Blue, graphics);
            DebugDrawer.DebugDrawRect(enemyInfo.GetGuessPositionHeading(10), 10, System.Drawing.Brushes.Green, graphics);
            DebugDrawer.DebugDrawRect(enemyInfo.GetGuessPositionEducated(10), 10, System.Drawing.Brushes.Red, graphics);

            DebugDrawer.DebugDrawLine(this.Position, enemyInfo.Bearings.GetCurrent() + HeadingRadians, 20, System.Drawing.Pens.Red, graphics);
            DebugDrawer.DebugDrawLine(this.Position, MiscHelper.GetAngleBetweenPositions(Position, enemyInfo.GetGuessPositionEducated(10)) , 20, System.Drawing.Pens.Green, graphics);
        }
    }



   

   

    
}
