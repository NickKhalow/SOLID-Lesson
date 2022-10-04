using SOLID.Princeples;


var consolePLayer = new DIP.DurakGame.ConsolePlayer("C#");
var botPlayer = new DIP.DurakGame.BotPlayer("Java");

var table = new DIP.DurakGame.Game(new DIP.DurakGame.AbstractPlayer[] {consolePLayer, botPlayer});

table.Start();