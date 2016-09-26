namespace RobotAPISample.Workflows
{
    public enum WorkflowType
    {
        CheckCitrixAvailable,
        CitrixDemo,
        ChromeDownloadCitrix,
        LINZTitleSearch
    }

    public static class WorkflowFiles
    {
        public static string CheckCitrixAvailableWorkflow = @"C:\Git\UiPathSDK\Code Samples\Robot API\RobotAPISample\Workflows\DetectExpiredLandonlineIcaFile2\Main.xaml";
        public static string CitrixDemoWorkflow = @"C:\Git\UiPathSDK\Code Samples\Robot API\RobotAPISample\Workflows\CitrixDemoWithVariables2\Main.xaml";
        public static string ChromeDownloadCitrixWorkflow = @"C:\Git\UiPathSDK\Code Samples\Robot API\RobotAPISample\Workflows\DownloadCitrixClientOnly.xaml";
        public static string LINZTitleSearchWorkflow = @"C:\Git\UiPathSDK\Code Samples\Robot API\RobotAPISample\Workflows\LINZTitleSearch\Main.xaml";
    }
}
