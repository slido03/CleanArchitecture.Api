using CleanArchitecture.Shared.Constants.Permission;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static CleanArchitecture.Shared.Constants.Application.ApplicationConstants.Formats;

namespace CleanArchitecture.Shared.Constants.Application
{
    public static partial class ApplicationConstants
    {
        public static class SignalR
        {
            public const string HubUrl = "/signalRHub";
            public const string SendUpdateDashboard = "UpdateDashboardAsync";
            public const string ReceiveUpdateDashboard = "UpdateDashboard";
            public const string SendRegenerateTokens = "RegenerateTokensAsync";
            public const string ReceiveRegenerateTokens = "RegenerateTokens";
            public const string ReceiveChatNotification = "ReceiveChatNotification";
            public const string SendChatNotification = "ChatNotificationAsync";
            public const string ReceiveMessage = "ReceiveMessage";
            public const string SendMessage = "SendMessageAsync";

            public const string OnConnect = "OnConnectAsync";
            public const string ConnectUser = "ConnectUser";
            public const string OnDisconnect = "OnDisconnectAsync";
            public const string DisconnectUser = "DisconnectUser";
            public const string OnChangeRolePermissions = "OnChangeRolePermissions";
            public const string LogoutUsersByRole = "LogoutUsersByRole";

            public const string PingRequest = "PingRequestAsync";
            public const string PingResponse = "PingResponseAsync";

        }
        public static partial class Cache
        {
            //Sgcpj cache keys
            public const string GetAllExternalApplicationsCacheKey = "all-external-applications";
            public const string GetAllDocumentTypesCacheKey = "all-document-types";
            public const string GetAllDocumentVersionsCacheKey = "all-document-versions";
            public const string GetAllDocumentMatchingsCacheKey = "all-document-matchings";

            public static string GetAllEntityExtendedAttributesCacheKey(string entityFullName)
            {
                return $"all-{entityFullName}-extended-attributes";
            }

            public static string GetAllEntityExtendedAttributesByEntityIdCacheKey<TEntityId>(string entityFullName, TEntityId entityId)
            {
                return $"all-{entityFullName}-extended-attributes-{entityId}";
            }
        }

        public static class MimeTypes
        {
            public const string OpenXml = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        //Formats
        public static class Formats
        {
            public static class DocumentFormats
            {
                public const string GetPDF = "PDF";
                public const string GetXLSX = "XLSX";
                public const string GetDOCX = "DOCX";
                public const string GetCSV = "CSV";
                public const string GetPPTX = "PPTX";

                public static bool CheckFormat(string format)
                {
                    format = format.ToUpper();
                    if ((format != GetPDF) && (format != GetXLSX) && (format != GetCSV) && (format != GetDOCX) && (format != GetPPTX))
                        return false;
                    else
                        return true;
                }

                public static string GetMatchingExtension(string format)
                {
                    if (!CheckFormat(format))
                        return string.Empty;
                    else
                        return $".{format.ToLower()}";
                }

                /// <summary>
                /// Returns a list of Document formats.
                /// </summary>
                /// <returns></returns>
                public static List<string> GetRegisteredFormats()
                {
                    var formats = new List<string>();
                    foreach (var prop in typeof(DocumentFormats).GetFields())
                    {
                        var propertyValue = prop.GetValue(null);
                        if (propertyValue is not null)
                            formats.Add(propertyValue.ToString());
                    }
                    return formats;
                }
            }

            public static class ImageFormats
            {
                public const string GetJPG = "JPG";
                public const string GetPNG = "PNG";

                public static bool CheckFormat(string format)
                {
                    format = format.ToUpper();
                    if ((format != GetJPG) && (format != GetPNG))
                        return false;
                    else
                        return true;
                }

                /// <summary>
                /// Returns a list of Image formats.
                /// </summary>
                /// <returns></returns>
                public static List<string> GetRegisteredFormats()
                {
                    var formats = new List<string>();
                    foreach (var prop in typeof(ImageFormats).GetFields())
                    {
                        var propertyValue = prop.GetValue(null);
                        if (propertyValue is not null)
                            formats.Add(propertyValue.ToString());
                    }
                    return formats;
                }
            }
        }

        //Extensions
        public static class Extensions
        {
            public static class DocumentExtensions
            {
                public const string GetPDF = ".pdf";
                public const string GetXLSX = ".xlsx";
                public const string GetDOCX = ".docx";
                public const string GetCSV = ".csv";
                public const string GetPPTX = ".pptx";

                public static bool CheckExtension(string extension)
                {
                    extension = extension.ToLower();
                    if ((extension != GetPDF) && (extension != GetXLSX) && (extension != GetCSV) && (extension != GetPPTX) && (extension != GetDOCX))
                        return false;
                    else
                        return true;
                }

                /// <summary>
                /// Returns a list of Document extensions.
                /// </summary>
                /// <returns></returns>
                public static List<string> GetRegisteredExtensions()
                {
                    var extensions = new List<string>();
                    foreach (var prop in typeof(DocumentExtensions).GetFields())
                    {
                        var propertyValue = prop.GetValue(null);
                        if (propertyValue is not null)
                            extensions.Add(propertyValue.ToString());
                    }
                    return extensions;
                }
            }

            public static class ImageExtensions
            {
                public const string GetJPG = ".jpg";
                public const string GetPNG = ".png";

                public static bool CheckExtension(string extension)
                {
                    extension = extension.ToLower();
                    if ((extension != GetJPG) && (extension != GetPNG))
                        return false;
                    else
                        return true;
                }

                /// <summary>
                /// Returns a list of Image extensions.
                /// </summary>
                /// <returns></returns>
                public static List<string> GetRegisteredExtensions()
                {
                    var extensions = new List<string>();
                    foreach (var prop in typeof(ImageExtensions).GetFields())
                    {
                        var propertyValue = prop.GetValue(null);
                        if (propertyValue is not null)
                            extensions.Add(propertyValue.ToString());
                    }
                    return extensions;
                }
            }
        }
    }
}