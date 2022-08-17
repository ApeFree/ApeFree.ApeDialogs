using System;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// Dialog构造工厂
    /// </summary>
    public sealed class DialogFactory
    {
        private static readonly Lazy<DialogFactory> lazy = new Lazy<DialogFactory>(() => new DialogFactory());
        public static DialogFactory Factory
        {
            get
            {
                return lazy.Value;
            }
        }
        private DialogFactory() { }
    }
}
