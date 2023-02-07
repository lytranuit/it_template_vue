using Vue.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Vue.Data
{
	public class AuthContext : DbContext
	{
		private IActionContextAccessor actionAccessor;
		private UserManager<UserModel> UserManager;
		public AuthContext(DbContextOptions<AuthContext> options, UserManager<UserModel> UserMgr, IActionContextAccessor ActionAccessor) : base(options)
		{
			actionAccessor = ActionAccessor;
			UserManager = UserMgr;
		}

		public DbSet<AuditTrailsModel> AuditTrailsModel { get; set; }

		public DbSet<UserModel> UserModel { get; set; }
		
		//public DbSet<User2Model> User2Model { get; set; }
		public DbSet<EmailModel> EmailModel { get; set; }
		public DbSet<TokenModel> TokenModel { get; set; }
		public DbSet<DepartmentModel> DepartmentModel { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers");

			//modelBuilder.Entity<DocumentModel>().HasMany(l => l.Teams).WithOne().HasForeignKey("LeagueId");
			//modelBuilder.Entity<ProcessTableDataModel>()
			// .Property(b => b._data).HasColumnName("data");

		}
		protected override void ConfigureConventions(ModelConfigurationBuilder builder)
		{
		}
		public override int SaveChanges()
		{
			OnBeforeSaveChanges();
			return base.SaveChanges();
		}
		private void OnBeforeSaveChanges()
		{
			ChangeTracker.DetectChanges();
			var auditEntries = new List<AuditEntry>();
			var user_http = actionAccessor.ActionContext.HttpContext.User;
			var user_id = UserManager.GetUserId(user_http);
			var changes = ChangeTracker.Entries();
			foreach (var entry in changes)
			{
				if (entry.Entity is AuditTrailsModel || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
					continue;

				var auditEntry = new AuditEntry(entry);
				auditEntry.TableName = entry.Entity.GetType().Name;
				auditEntry.UserId = user_id;
				auditEntries.Add(auditEntry);
				foreach (var property in entry.Properties)
				{

					string propertyName = property.Metadata.Name;
					if (property.Metadata.IsPrimaryKey())
					{
						auditEntry.KeyValues[propertyName] = property.CurrentValue;
						continue;
					}
					switch (entry.State)
					{
						case EntityState.Added:
							auditEntry.AuditType = AuditType.Create;
							auditEntry.NewValues[propertyName] = property.CurrentValue;
							break;
						case EntityState.Deleted:
							auditEntry.AuditType = AuditType.Delete;
							auditEntry.OldValues[propertyName] = property.OriginalValue;
							break;
						case EntityState.Modified:
							if (property.IsModified)
							{
								var Original = entry.GetDatabaseValues().GetValue<object>(propertyName);
								var Current = property.CurrentValue;
								if (JsonConvert.SerializeObject(Original) == JsonConvert.SerializeObject(Current))
									continue;
								auditEntry.ChangedColumns.Add(propertyName);
								auditEntry.AuditType = AuditType.Update;
								auditEntry.OldValues[propertyName] = Original;
								auditEntry.NewValues[propertyName] = Current;

							}
							break;
					}

				}
			}
			foreach (var auditEntry in auditEntries)
			{
				AuditTrailsModel.Add(auditEntry.ToAudit());
			}
		}
	}
}
