using System.Data;
using Dapper;

namespace Backend.Utils;

public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
        => parameter.Value = value.ToDateTime(TimeOnly.MinValue);

    public override DateOnly Parse(object value)
        => DateOnly.FromDateTime((DateTime)value);
}

public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
        => parameter.Value = value;

    public override TimeSpan Parse(object value)
        => (TimeSpan)value;
}
