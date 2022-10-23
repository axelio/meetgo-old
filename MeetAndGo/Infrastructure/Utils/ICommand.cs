using System.Threading.Tasks;

namespace MeetAndGo.Infrastructure.Utils
{
    public interface ICommand<T> { }
    public interface ICommand { }

    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<Result<TResult>> Handle(TCommand command);
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task<Result> Handle(TCommand command);
    }
}
