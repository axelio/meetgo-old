using System.Threading.Tasks;

namespace MeetAndGo.Infrastructure.Utils
{
    public interface IQuery { }
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery
    {
        Task<TResult> Handle(TQuery query);
    }
}
