namespace Haus.Cqrs.Commands;

public interface ICommand { }

public interface ICommand<out TResult> { }
