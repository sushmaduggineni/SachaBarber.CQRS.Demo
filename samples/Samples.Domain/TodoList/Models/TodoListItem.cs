﻿namespace Samples.Domain.TodoList
{
    public class TodoListItem
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public TodoItemStatus Status { get; set; }
    }
}