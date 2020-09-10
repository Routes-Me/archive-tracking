using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveTrackService.Models
{
    public class CommonMessage
    {
        public static string BadRequest = "Provide valid data.";
        public static string FeedNotFound = "Feed not found.";
        public static string FeedRetrived = "Feed retrived successfully.";
        public static string FeedInsert = "Feed inserted successfully.";
        public static string FeedUpdate = "Feed updated successfully.";
        public static string FeedDelete = "Feed deleted successfully.";
        public static string ExceptionMessage = "Something went wrong. Error Message - ";
    }
}
