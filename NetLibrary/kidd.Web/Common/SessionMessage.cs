using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace kidd.Web.Common
{
    public class SessionMessage
    {
        private static string title_key = "session_message_title_key";
        private static string content_key = "session_message_content_key";
        private static string type_key = "session_message_type_key";
        private static string display_type_key = "session_message_display_type_key";

        private HttpSessionStateBase session = null;
        //private SessionMessageDisplayType DisplayType;

        public SessionMessage(HttpSessionState session)
        {
            this.session = new HttpSessionStateWrapper(session);
        }

        public SessionMessage(HttpSessionStateBase session)
        {
            this.session = session;
        }


        public void Set(SessionMessageDisplayType displayType, string content, string type, string title = "")
        {
            this.session[title_key] = title;
            this.session[content_key] = content;
            this.session[type_key] = type;
            this.session[display_type_key] = displayType;
        }

        public bool HasMessage()
        {
            object value = this.session[content_key];
            return value != null;
        }

        public string GetTitle()
        {
            var result = GetSessionValueThenDelete(title_key);
            return result == null ? "" : result.ToString();
        }

        public string GetContent()
        {
            var result = GetSessionValueThenDelete(content_key);
            return result == null ? "" : result.ToString();
        }

        public new string GetType()
        {
            var result = GetSessionValueThenDelete(type_key);
            return result == null ? "" : result.ToString();
        }

        public SessionMessageDisplayType GetDisplayType()
        {
            var result = GetSessionValueThenDelete(display_type_key);
            return result == null ? SessionMessageDisplayType.Notification : (SessionMessageDisplayType)Enum.Parse(typeof(SessionMessageDisplayType), result.ToString());
        }


        private object GetSessionValueThenDelete(string key)
        {
            object value = this.session[key];
            this.session.Remove(key);
            return value;
        }

    }

    public enum SessionMessageDisplayType
    {
        Notification = 1,
        Alert = 2,
    }
}
