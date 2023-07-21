namespace Todo.Microservices;

public record class TodoMsApiResponse<T>(bool Error, T Data);
