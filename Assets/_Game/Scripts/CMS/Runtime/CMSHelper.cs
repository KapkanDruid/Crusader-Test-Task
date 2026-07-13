namespace Game.CMS.Runtime
{
    public static class CMSHelper
    {
        public static string GetEntityId<T>() where T : CMSEntity
        {
            return typeof(T).FullName;
        }
    }
}
