using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Helpers
{
    public class MessageHelper
    {
        public static TMessage FromData<TMessage>(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            return (TMessage)JsonConvert.DeserializeObject<TMessage>(json);
        }
    }
}
