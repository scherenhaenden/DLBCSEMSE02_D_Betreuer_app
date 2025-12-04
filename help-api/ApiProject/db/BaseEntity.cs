using System;

namespace ApiProject.Db;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

