namespace GameJamSadConsoleSample
{
    public class HUD
    {
        public string[] HUDViewList =
        {
            "------------------------------------------------------------------------------------------",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "|                                                                           |   BATTERY   |",
            "|                                                                           |             |",
            "|                                                                           |     _nn_    |",
            "|                                                                           |    |$$$$|   |",
            "|                                                                           |    |$$$$|   |",
            "|                                                                           |    |$$$$|   |",
            "|                                                                           |    |$$$$|   |",
            "|                                                                           |    '----'   |",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "|                                                                           |    ITEMS    |",
            "|                                                                           |             |",
            "|                                                                           |   0  /  5   |",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "|                                                                           |    TIME     |",
            "|                                                                           |             |",
            "|                                                                           |   12 : 35   |",
            "|                                                                           |             |",
            "|                                                                           |             |",
            "-------------------------------------------------------------------------------------------"
        };

        private string emptyBatteryLine =
            "|                                                                           |    |    |   |";
        private string fullBatteryLine =
            "|                                                                           |    |$$$$|   |";

        public string GetBatteryLine(int currentLineLevel, int batteryLevel)
        {
            return batteryLevel >= currentLineLevel ? fullBatteryLine : emptyBatteryLine;
        }
        public string GetItemsLine(int numberOfItemsPickedUp)
        {
            return "|                                                                           |   " + numberOfItemsPickedUp + "  /  5   |";
        }

        public string GetTimerLine(string minutes, string seconds)
        {
            return "|                                                                           |   "+minutes+" : "+seconds+"   |";
        }
    }
}