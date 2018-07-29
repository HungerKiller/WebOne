using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }

        // 在这里可以省略 DbSet<Enrollment> 和 DbSet<Course> 语句，实现的功能没有任何改变。 
        // Entity Framework 会隐式包含这两个实体因为 Student 实体引用了 Enrollment 实体、Enrollment 实体引用了 Course 实体
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }

        // 对 DbContext 指定单数的表名来覆盖默认的表名
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Course>().ToTable("Course");
        //    modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
        //    modelBuilder.Entity<Student>().ToTable("Student");
        //}
    }
}