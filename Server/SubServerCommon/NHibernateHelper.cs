using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace SubServerCommon
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public NHibernateHelper()
        {
            InitializeSessionFactory();
        }

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    InitializeSessionFactory();
                return _sessionFactory;
            }
        }

        private static void InitializeSessionFactory()
        {
            _sessionFactory = Fluently.Configure().Database(
                MySQLConfiguration.Standard
                    .ConnectionString(cs => cs.Server("158.129.18.169")
                        .Database("mayhemandhell2")
                        .Username("root")
                        .Password("")))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateHelper>())
                .BuildSessionFactory();
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}