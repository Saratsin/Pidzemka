namespace Pidzemka.Common
{
    public static class Constants
    {
        public const string SubwayRouteLinePartsRegexPattern = "(<.*?id=\\\"line_part_({0})\\\".*?fill=\\\")#\\w*?(\\\".*?(/>|</.*?>))";

        public const string SubwayRouteLinePartsReplaceRegex = "$1#AAAAAA$3";
    }
}