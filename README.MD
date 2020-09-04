#### 背景
17年开始，公司开始向DotNet Core转型，面对ORM工具的选型，当时围绕Dapper和EF发生了激烈的讨论。项目团队更加关注快速交付，他们主张使用EF这种能快速开发的ORM工具;而在线业务团队对性能有更高的要求，他们更希望使用能直接执行Sql语句的Dapper这样可控性更高。而对于架构团队来说，满足开发团队的各种需求，提高他们的开发效率是最核心的价值所在，所以当时决定做一个混合型的既支持EF又支持dapper的数据仓储。

#### 为什么选择EF+Dapper
目前来说EF和Dapper是.NET平台最主流的ORM工具，团队成员的接受程度很高，学习成本最低，因为主主流，所以相关的资料非常齐全，各种坑也最少。

#### 介绍
1. 它不是一个ORM工具，它不做任何关于数据底层的操作
2. 它是一个简易封装的数据库仓储和工作单元模型
3. 能帮助你快速的构建项目的数据访问层
4. 经过了2年多时间，10个项目组，大小近100多个线上项目的考验
5. 支持EF和Dapper，可以在项目中随意切换使用
6. 支持工作单元模式，也支持事务
7. 支持Mysql和Mssql
8. 支持同步和异步操作，推荐使用异步
> PS: 简单操作使用EF，复杂sql操作使用Dapper是快速开发的秘诀。

#### 使用方法
引入nuget
```
<PackageReference Include="Leo.Chimp" Version="2.1.2" />
```
创建实体对象，继承IEntity

```
public class School : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```
创建仓储接口和实现类，分别继承IRepository和EfCoreRepository
```
public interface ISchoolRepository : IRepository<School>
{
}
public class SchoolRepository: EfCoreRepository<School>,ISchoolRepository
{
    public SchoolRepository(DbContext context) : base(context)
    {
    }
}
```
创建上下文，继承BaseDbContext，如果你不需要操作上下文可以不用做这一步
```
public class ChimpDbContext : BaseDbContext
{
    public ChimpDbContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //your code
    }
}
```
注入服务
```
services.AddChimp<ChimpDbContext>(
                opt =>
                opt.UseSqlServer("Server=10.0.0.99;Database=chimp;Uid=sa;Pwd=Fuluerp123")
);
```
如果你没有创建上下文
```
services.AddChimp(
                opt =>
                opt.UseSqlServer("Server=10.0.0.99;Database=chimp;Uid=sa;Pwd=Fuluerp123")
);
```
在Controller中使用
```
public class ValuesController : ControllerBase
{
    private readonly ISchoolRepository _schoolRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ValuesController(ISchoolRepository schoolRepository, IUnitOfWork unitOfWork)
    {
        _schoolRepository = schoolRepository;
        _unitOfWork = unitOfWork;
    }
}
```
#### 详细使用说明
查询
```
//根据主键查询
_schoolRepository.GetById(Id)
```
```
//不带追踪的查询，返回数据不能用于更新或删除操作，性能快
schoolRepository.TableNoTracking.First(x => x.Id == Id);
```
```
//带追踪的查询，返回数据可以用于更新或删除操作，性能稍慢
schoolRepository.Table.First(x => x.Id == Id);
```
```
//分页查询
_schoolRepository.TableNoTracking.ToPagedList(1,10);
```
```
//sql语句查询
_unitOfWork.QueryAsync<School>("select * from school");

```
```
//sql分页查询
_unitOfWork.QueryPagedListAsync<School>(1, 10, "select * from school order by id");
```
> 关于查询，暴露了返回IQueryable的TableNoTracking、Table这两个属性，让开发人员自己组装Lambda表达式进行查询操作

新增
```
//新增，支持批量新增
_schoolRepository.Insert(school);
_unitOfWork.SaveChanges();
```
```
//sql语句新增
_unitOfWork.ExecuteAsync("insert school(id,name) values(@Id,@Name)",
                        school);
```
编辑
```
//编辑，支持批量编辑
var school = _schoolRepository.GetByIdAsync(Id);
school.Name="newschool";
_schoolRepository.Update(school);
_unitOfWork.SaveChanges();
```
```
//编辑，不用先查询
var school = new School
{
    Id = "xxxxxx",
    Name = "newschool"
};
_schoolRepository.Update(school, x => x.Name);
_unitOfWork.SaveChanges();
```
```
//sql语句编辑
_unitOfWork.ExecuteAsync("update school set name=@Name where id=@Id",
                        school);
```
删除
```
//删除，支持批量删除
_schoolRepository.Delete(school);
_unitOfWork.SaveChanges();
```
```
//根据lambda删除
_schoolRepository.Delete(x => x.Id == Id);
_unitOfWork.SaveChanges();
```
事务
```
//工作单元模式使用事务
await _schoolRepository.InsertAsync(school1);
await _schoolRepository.InsertAsync(school1);
await _unitOfWork.SaveChangesAsync();
```
```
//dapper使用事务
using (var tran = _unitOfWork.BeginTransaction())
{
    try
    {
        await _unitOfWork.ExecuteAsync("insert school(id,name) values(@Id,@Name)",
            school1,tran);
        await _unitOfWork.ExecuteAsync("insert school(id,name) values(@Id,@Name)",
            school2,tran);
        tran.Commit();
    }
    catch (Exception e)
    {
        tran.Rollback();
    }
}
```
```
//dapper+ef混合使用事务
using (var tran = _unitOfWork.BeginTransaction())
{
    try
    {
        await _schoolRepository.InsertAsync(school1);
        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.ExecuteAsync("insert school(id,name) values(@Id,@Name)",
            school2);
        tran.Commit();
    }
    catch (Exception e)
    {
        tran.Rollback();
    }
}
```
高级用法
```
//通过GetConnection可以使用更多dapper扩展的方法
_unitOfWork.GetConnection().QueryAsync("select * from school");
```
#### 写在最后
Chimp核心是基于EF和Dapper的，所以EF和Dapper一切功能都可以使用。比如导航属性，字段映射等等。这个库是线上项目核心依赖，会长期更新维护，希望大家能提出更好的意见。

#### 项目地址
数据库脚本在根目录的sqlscript文件夹里面  
[github地址](https://github.com/longxianghui/chimp.git)
