using System;
using RobotAPISample.Automation;

namespace RobotAPISample
{
    class Program
    {
        static void Main(string[] args)
        {
            var citrixIsRunning = Landonline.EnsureCitrixRunning();
            if (!citrixIsRunning)
            {
                throw new ApplicationException("Unable to launch Citrix");
            }

            // TODO must ensure that it handles invalid references gracefully
            //TitleReferences = new List<string> { "WN516/98", "OT17A/765", "MB23C/987" },
            //TitleReferences = new [] { "WN516/98", "OT17A/765" },

            var response = Landonline.ExecuteTitleSearch(new TitleSearchRequest[] 
            {
                new TitleSearchRequest
                {
                    TitleReference = "invalid",
                    Type = Workflows.LINZTitleSearchType.TitleSearchNoDiagram,
                    OrderId = "Order000"
                },
                //new TitleSearchRequest
                //{
                //    TitleReference = "WN516/98",
                //    Type = Workflows.LINZTitleSearchType.Guaranteed,
                //    OrderId = "Order001"
                //},
                //new TitleSearchRequest
                //{
                //    TitleReference = "invalid",
                //    Type = Workflows.LINZTitleSearchType.TitleSearchNoDiagram,
                //    OrderId = "Order000"
                //},
                //new TitleSearchRequest
                //{
                //    TitleReference = "OT17A/765",
                //    Type = Workflows.LINZTitleSearchType.Historical,
                //    OrderId = "Order002"
                //}
            });
            
            Console.ReadLine();
        }
    }
    
}

