using CodeRunner.Resources.Scripts;
using System.Diagnostics;

namespace CodeRunner
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("game", typeof(GamePage));

            Enemies.Ghost.BlindMovementScript += (sender, e) =>
                {
                    if (sender is Enemy enemy && GamePage.Instance != null)
                    {
                        var player = GamePage.Instance.PlayerRef; // Need a reference to the player
                        var dx = player.Location.X - enemy.Location.X;
                        var dy = player.Location.Y - enemy.Location.Y;
                        var length = Math.Sqrt(dx * dx + dy * dy);

                        if (length > 0)
                        {
                            enemy.Location = new Point(
                                enemy.Location.X + (float)(dx / length * enemy.Speed),
                                enemy.Location.Y + (float)(dy / length * enemy.Speed)
                            );
                        }
                    }
                };

            Enemies.Dummy.BlindMovementScript += (sender, e) =>
            {
                if (sender is not Enemy enemy) return;

                var player = GamePage.Instance.PlayerRef;

                var dx = player.Location.X - enemy.Location.X;
                var dy = player.Location.Y - enemy.Location.Y;
                var length = Math.Sqrt(dx * dx + dy * dy);
                if (length < 3) return;

                var moveX = (float)(dx / length * enemy.Speed);
                var moveY = (float)(dy / length * enemy.Speed);

                // Try moving X
                var tryX = new Point(enemy.Location.X + moveX, enemy.Location.Y);
                if (!GamePage.ContainSurroundings((float)tryX.X, (float)tryX.Y, enemy.HitBoxRadius, 3))
                    enemy.Location = new Point(tryX.X, enemy.Location.Y);

                // Try moving Y
                var tryY = new Point(enemy.Location.X, enemy.Location.Y + moveY);
                if (!GamePage.ContainSurroundings((float)(float)tryY.X, (float)tryY.Y, enemy.HitBoxRadius, 3))
                    enemy.Location = new Point(enemy.Location.X, tryY.Y);

                Debug.WriteLine(enemy.HitBoxRadius);
            };
    }
}

}
