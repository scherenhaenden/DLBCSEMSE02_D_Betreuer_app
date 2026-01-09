using ApiProject.DatabaseAccess.Entities;
using ApiProject.Seed.Services;
using Bogus;
using System.Text.Json;
using System.Text;
using ApiProject.Constants;
using Microsoft.EntityFrameworkCore;
using ApiProject.DatabaseAccess.Context;

namespace ApiProject.Tests.CreateSeed;

/// <summary>
/// Extended UserDataAccessModel for seeding purposes to include the plain text password.
/// This helps in testing login functionality with known credentials.
/// </summary>
public class UserSeed: UserDataAccessModel
{
    public required string Password { get; set; }
}

public class SeedData
{
    public List<RoleDataAccessModel> Roles { get; set; }
    public List<BillingStatusDataAccessModel> BillingStatuses { get; set; }
    public List<ThesisStatusDataAccessModel> ThesisStatuses { get; set; }
    public List<RequestTypeDataAccessModel> RequestTypes { get; set; }
    public List<RequestStatusDataAccessModel> RequestStatuses { get; set; }
    public List<ThesisOfferStatusDataAccessModel> ThesisOfferStatuses { get; set; }
    public List<SubjectAreaDataAccessModel> Topics { get; set; }
    public List<UserSeed> Users { get; set; }
    public List<UserRoleDataAccessModel> UserRoles { get; set; }
    public List<UserToSubjectAreas> UserTopics { get; set; }
    public List<ThesisOfferDataAccessModel> ThesisOffers { get; set; }
    public List<ThesisOfferApplicationDataAccessModel> ThesisOfferApplications { get; set; }
    public List<ThesisDataAccessModel> Theses { get; set; }
    public List<ThesisDocumentDataAccessModel> ThesisDocuments { get; set; }
    public List<ThesisRequestDataAccessModel> ThesisRequests { get; set; }
}


[TestFixture]
public class Seeder
{
   public List<ThesisRequestDataAccessModel> ThesisRequests { get; set; }
    

    //[Test]
    public SeedData CreateSeed()
    {
        
        ICreateSeedOfObject createSeedOfObject = new CreateSeedOfObject();
        
        // --- 1. Roles ---
        var roleNames = new[] { Roles.Student, Roles.Tutor, Roles.Admin };
        var rolesToSeed = new List<RoleDataAccessModel>();
        foreach (var roleName in roleNames)
        {
            rolesToSeed.Add(new Faker<RoleDataAccessModel>()
                .RuleFor(r => r.Id, _ => Guid.NewGuid())
                .RuleFor(r => r.Name, roleName)
                .RuleFor(r => r.CreatedAt, f => f.Date.Recent())
                .RuleFor(r => r.UpdatedAt, (_, r) => r.CreatedAt)
                .Generate());
        }

        // --- 2. Billing Statuses ---
        var billingStatusNames = new[] { BillingStatuses.None, BillingStatuses.Issued, BillingStatuses.Paid };
        var billingStatusesToSeed = new List<BillingStatusDataAccessModel>();
        foreach (var statusName in billingStatusNames)
        {
            billingStatusesToSeed.Add(new Faker<BillingStatusDataAccessModel>()
                .RuleFor(b => b.Id, _ => Guid.NewGuid())
                .RuleFor(b => b.Name, statusName)
                .RuleFor(b => b.CreatedAt, f => f.Date.Recent())
                .RuleFor(b => b.UpdatedAt, (_, b) => b.CreatedAt)
                .Generate());
        }

        // --- 3. Thesis Statuses ---
        var thesisStatusNames = new[] { ThesisStatuses.InDiscussion, ThesisStatuses.Registered, ThesisStatuses.Submitted, ThesisStatuses.Defended };
        var thesisStatusesToSeed = new List<ThesisStatusDataAccessModel>();
        foreach (var statusName in thesisStatusNames)
        {
            thesisStatusesToSeed.Add(new Faker<ThesisStatusDataAccessModel>()
                .RuleFor(t => t.Id, _ => Guid.NewGuid())
                .RuleFor(t => t.Name, statusName)
                .RuleFor(t => t.CreatedAt, f => f.Date.Recent())
                .RuleFor(t => t.UpdatedAt, (_, t) => t.CreatedAt)
                .Generate());
        }

        // --- 4. Request Types & Statuses ---
        var requestTypesToSeed = new List<RequestTypeDataAccessModel>
        {
            new() { Id = Guid.NewGuid(), Name = RequestTypes.Supervision, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = RequestTypes.CoSupervision, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        var requestStatusesToSeed = new List<RequestStatusDataAccessModel>
        {
            new() { Id = Guid.NewGuid(), Name = RequestStatuses.Pending, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = RequestStatuses.Accepted, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = RequestStatuses.Rejected, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        // --- 5. Topics ---
        var topicsFromMethod = TopicForSeeding();
        var topicsToSeed = new List<SubjectAreaDataAccessModel>();
        foreach (var topicItem in topicsFromMethod)
        {
            topicsToSeed.Add(new Faker<SubjectAreaDataAccessModel>()
                .RuleFor(t => t.Id, _ => Guid.NewGuid())
                .RuleFor(t => t.Title, topicItem.Title)
                .RuleFor(t => t.Description, topicItem.Description)
                .RuleFor(t => t.CreatedAt, f => f.Date.Recent())
                .RuleFor(t => t.UpdatedAt, (_, t) => t.CreatedAt)
                .Generate());
        }

        // --- 6. Users ---
        var userFaker = new Faker<UserSeed>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => $"user{f.IndexFaker}@example.com")
            .RuleFor(u => u.Password, f => f.Internet.Password())
            .RuleFor(u => u.PasswordHash, (_, u) => BCrypt.Net.BCrypt.HashPassword(u.Password, workFactor: 4))
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.CreatedAt, f => f.Date.Past())
            .RuleFor(u => u.UpdatedAt, f => f.Date.Recent());
        
        var users = userFaker.Generate(100);

        // --- 7. Assign Roles ---
        var userRolesToSeed = new List<UserRoleDataAccessModel>();
        var faker = new Faker();
        foreach (var user in users)
        {
            var randomRole = faker.PickRandom(rolesToSeed);
            userRolesToSeed.Add(new UserRoleDataAccessModel { UserId = user.Id, RoleId = randomRole.Id });
        }
        
        // Specific Testers
        var testers = new[] { "Abraham", "Eddie", "Stefan", "Michael" };
        foreach (var testerName in testers)
        {
            var password = "password123";
            foreach (var role in rolesToSeed)
            {
                var specificUser = new UserSeed
                {
                    Id = Guid.NewGuid(),
                    FirstName = testerName,
                    LastName = role.Name,
                    Email = $"{testerName.ToLower()}.{role.Name.ToLower()}@test.com",
                    Password = password,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor:4),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                users.Add(specificUser);
                userRolesToSeed.Add(new UserRoleDataAccessModel { UserId = specificUser.Id, RoleId = role.Id });
            }
        }
        
        // --- 8. Tutors & Topics Assignment ---
        var tutorRole = rolesToSeed.First(r => r.Name == Roles.Tutor);
        var tutorUserIds = userRolesToSeed.Where(ur => ur.RoleId == tutorRole.Id).Select(ur => ur.UserId);
        
        var tutors = users.Where(u => tutorUserIds.Contains(u.Id)).ToList();
        
        var userTopicAssignments = new HashSet<(Guid UserId, Guid TopicId)>();

        // Assign 2-7 topics to each tutor
        foreach (var tutor in tutors)
        {
            var numTopics = faker.Random.Int(2, 7);
            var topicsToAssign = faker.PickRandom(topicsToSeed, numTopics);
            foreach (var topic in topicsToAssign) userTopicAssignments.Add((tutor.Id, topic.Id));
        }

        // Ensure coverage (min 4 tutors per topic)
        foreach (var topic in topicsToSeed)
        {
            var assignmentsForTopic = userTopicAssignments.Count(ut => ut.TopicId == topic.Id);
            var needed = 4 - assignmentsForTopic;
            if (needed > 0)
            {
                var assignedTutorIds = userTopicAssignments.Where(ut => ut.TopicId == topic.Id).Select(ut => ut.UserId);
                var assignableTutors = tutors.Where(t => !assignedTutorIds.Contains(t.Id)).ToList();
                var tutorsToAssign = faker.PickRandom(assignableTutors, Math.Min(needed, assignableTutors.Count));
                foreach (var tutor in tutorsToAssign) userTopicAssignments.Add((tutor.Id, topic.Id));
            }
        }

        var userTopicsToSeed = userTopicAssignments
            .Select(ut => new UserToSubjectAreas { UserId = ut.UserId, SubjectAreaId = ut.TopicId })
            .ToList();
        
        // --- 9. Theses & Requests Generation ---
        var studentRole = rolesToSeed.First(r => r.Name == Roles.Student);
        var studentUserIds = userRolesToSeed.Where(ur => ur.RoleId == studentRole.Id).Select(ur => ur.UserId);
        var students = users.Where(u => studentUserIds.Contains(u.Id)).ToList();
        
        var thesesToSeed = new List<ThesisDataAccessModel>();
        var thesisRequestsToSeed = new List<ThesisRequestDataAccessModel>();
        var thesisDocumentsToSeed = new List<ThesisDocumentDataAccessModel>();

        var thesisFaker = new Faker<ThesisDataAccessModel>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.Title, f => f.Lorem.Sentence(3))

            .RuleFor(t => t.Description, f => f.Lorem.Sentence(1))

            .RuleFor(t => t.BillingStatusId, f => f.PickRandom(billingStatusesToSeed).Id)
            .RuleFor(t => t.CreatedAt, f => f.Date.Past())
            .RuleFor(t => t.UpdatedAt, f => f.Date.Recent());

        var thesisRequestFaker = new Faker<ThesisRequestDataAccessModel>()
            .RuleFor(tr => tr.Id, _ => Guid.NewGuid())
            .RuleFor(tr => tr.Message, f => f.Lorem.Sentence())
            .RuleFor(tr => tr.CreatedAt, f => f.Date.Past())
            .RuleFor(tr => tr.UpdatedAt, f => f.Date.Recent());

        var thesisDocumentFaker = new Faker<ThesisDocumentDataAccessModel>()
            .RuleFor(td => td.Id, _ => Guid.NewGuid())
            .RuleFor(td => td.FileName, f => f.System.FileName("pdf"))
            .RuleFor(td => td.ContentType, "application/pdf")
            .RuleFor(td => td.Content, _ => Encoding.UTF8.GetBytes("This is a thesis."))
            .RuleFor(td => td.CreatedAt, f => f.Date.Past())
            .RuleFor(td => td.UpdatedAt, f => f.Date.Recent());

        List<UserSeed> GetTutorsForTopic(Guid topicId)
        {
            var eligibleIds = userTopicAssignments.Where(ut => ut.TopicId == topicId).Select(ut => ut.UserId);
            return tutors.Where(t => eligibleIds.Contains(t.Id)).ToList();
        }

        // --- Scenario A: Established Theses (Accepted Requests) ---
        for (int i = 0; i < 40; i++)
        {
            var thesis = thesisFaker.Generate();
            thesis.OwnerId = faker.PickRandom(students).Id;
            thesis.StatusId = faker.PickRandom(thesisStatusesToSeed.Where(s => s.Name != ThesisStatuses.InDiscussion)).Id;
            
            var topic = faker.PickRandom(topicsToSeed);
            thesis.SubjectAreaId = topic.Id;

            var eligibleTutors = GetTutorsForTopic(topic.Id);
            var tutor = faker.PickRandom(eligibleTutors);
            thesis.TutorId = tutor.Id;

            if (faker.Random.Bool(0.3f))
            {
                var otherTutors = eligibleTutors.Where(t => t.Id != tutor.Id).ToList();
                if(otherTutors.Any())
                    thesis.SecondSupervisorId = faker.PickRandom(otherTutors).Id;
            }

            thesesToSeed.Add(thesis);

            var doc = thesisDocumentFaker.Generate();
            doc.ThesisId = thesis.Id;
            thesisDocumentsToSeed.Add(doc);

            var req = thesisRequestFaker.Generate();
            req.RequesterId = thesis.OwnerId;
            req.ReceiverId = thesis.TutorId.Value;
            req.ThesisId = thesis.Id;
            req.RequestTypeId = requestTypesToSeed.First(rt => rt.Name == RequestTypes.Supervision).Id;
            req.StatusId = requestStatusesToSeed.First(rs => rs.Name == RequestStatuses.Accepted).Id;
            thesisRequestsToSeed.Add(req);

            if (thesis.SecondSupervisorId.HasValue)
            {
                var coReq = thesisRequestFaker.Generate();
                coReq.RequesterId = thesis.TutorId.Value;
                coReq.ReceiverId = thesis.SecondSupervisorId.Value;
                coReq.ThesisId = thesis.Id;
                coReq.RequestTypeId = requestTypesToSeed.First(rt => rt.Name == RequestTypes.CoSupervision).Id;
                coReq.StatusId = requestStatusesToSeed.First(rs => rs.Name == RequestStatuses.Accepted).Id;
                thesisRequestsToSeed.Add(coReq);
            }
        }

        // --- Scenario B: Pending Negotiations (New Theses) ---
        for (int i = 0; i < 10; i++)
        {
            var thesis = thesisFaker.Generate();
            thesis.OwnerId = faker.PickRandom(students).Id;
            thesis.StatusId = thesisStatusesToSeed.First(s => s.Name == ThesisStatuses.InDiscussion).Id;
            thesis.TutorId = null; // CORRECTED: TutorId is null until request is accepted
            
            var topic = faker.PickRandom(topicsToSeed);
            thesis.SubjectAreaId = topic.Id;

            var eligibleTutors = GetTutorsForTopic(topic.Id);
            var proposedTutor = faker.PickRandom(eligibleTutors);

            thesesToSeed.Add(thesis);

            var req = thesisRequestFaker.Generate();
            req.RequesterId = thesis.OwnerId;
            req.ReceiverId = proposedTutor.Id;
            req.ThesisId = thesis.Id;
            req.RequestTypeId = requestTypesToSeed.First(rt => rt.Name == RequestTypes.Supervision).Id;
            req.StatusId = requestStatusesToSeed.First(rs => rs.Name == RequestStatuses.Pending).Id;
            req.Message = "Dear Professor, I would like to write my thesis about " + topic.Title;
            thesisRequestsToSeed.Add(req);

            if (i < 5)
            {
                var otherEligibleTutors = eligibleTutors.Where(t => t.Id != proposedTutor.Id).ToList();
                if (otherEligibleTutors.Any())
                {
                    var rejectedTutor = faker.PickRandom(otherEligibleTutors);
                    var rejectedReq = thesisRequestFaker.Generate();
                    rejectedReq.RequesterId = thesis.OwnerId;
                    rejectedReq.ReceiverId = rejectedTutor.Id;
                    rejectedReq.ThesisId = thesis.Id;
                    rejectedReq.RequestTypeId = requestTypesToSeed.First(rt => rt.Name == RequestTypes.Supervision).Id;
                    rejectedReq.StatusId = requestStatusesToSeed.First(rs => rs.Name == RequestStatuses.Rejected).Id;
                    rejectedReq.Message = "I am interested in your topic.";
                    rejectedReq.CreatedAt = DateTime.Now.AddMonths(-1);
                    thesisRequestsToSeed.Add(rejectedReq);
                }
            }
        }
        
        // --- X. Thesis Offer Statuses ---
        var thesisOfferStatusesToSeed = new List<ThesisOfferStatusDataAccessModel>
        {
            new() { Id = Guid.NewGuid(), Name = ThesisOfferStatuses.Open, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = ThesisOfferStatuses.Closed, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = ThesisOfferStatuses.Archived, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        
        var openOfferStatusId = thesisOfferStatusesToSeed.First(s => s.Name == ThesisOfferStatuses.Open).Id;
        var tutorsWithOffers = tutors
            .Where(_ => faker.Random.Bool(0.35f))
            .ToList();
        
        var thesisOffersToSeed = new List<ThesisOfferDataAccessModel>();
        
        foreach (var tutor in tutorsWithOffers)
        {
            // Subject areas this tutor supervises
            var tutorAreas = userTopicAssignments
                .Where(ut => ut.UserId == tutor.Id)
                .Select(ut => ut.TopicId)
                .ToList();

            if (!tutorAreas.Any())
                continue;

            // Each tutor offers 1–3 topics
            var offerCount = faker.Random.Int(1, 3);

            for (int i = 0; i < offerCount; i++)
            {
                thesisOffersToSeed.Add(new ThesisOfferDataAccessModel
                {
                    Id = Guid.NewGuid(),
                    Title = faker.Lorem.Sentence(5),
                    Description = faker.Lorem.Paragraph(),
                    TutorId = tutor.Id,
                    SubjectAreaId = faker.PickRandom(tutorAreas),
                    ThesisOfferStatusId = openOfferStatusId,
                    MaxStudents = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-faker.Random.Int(1, 120)),
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
        
        var thesisOfferApplicationsToSeed = new List<ThesisOfferApplicationDataAccessModel>();

        foreach (var offer in thesisOffersToSeed)
        {
            // 0–4 students apply
            var applicants = faker.PickRandom(students, faker.Random.Int(0, 4));

            foreach (var student in applicants)
            {
                thesisOfferApplicationsToSeed.Add(new ThesisOfferApplicationDataAccessModel
                {
                    Id = Guid.NewGuid(),
                    ThesisOfferId = offer.Id,
                    StudentId = student.Id,
                    RequestStatusId = faker.PickRandom(
                        requestStatusesToSeed.Where(rs =>
                            rs.Name == RequestStatuses.Pending || rs.Name == RequestStatuses.Rejected
                        )).Id,
                    Message = faker.Lorem.Sentence(),
                    CreatedAt = DateTime.UtcNow.AddDays(-faker.Random.Int(1, 30)),
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        // --- 13. Seed into Database ---
        
        var seedData = new SeedData
        {
            Roles = rolesToSeed,
            BillingStatuses = billingStatusesToSeed,
            ThesisStatuses = thesisStatusesToSeed,
            RequestTypes = requestTypesToSeed,
            RequestStatuses = requestStatusesToSeed,
            ThesisOfferStatuses = thesisOfferStatusesToSeed,
            Topics = topicsToSeed,
            Users = users,
            UserRoles = userRolesToSeed,
            UserTopics = userTopicsToSeed,
            ThesisOffers = thesisOffersToSeed,
            ThesisOfferApplications = thesisOfferApplicationsToSeed,
            Theses = thesesToSeed,
            ThesisDocuments = thesisDocumentsToSeed,
            ThesisRequests = thesisRequestsToSeed
        };
        
        
        var json = JsonSerializer.Serialize(seedData, new JsonSerializerOptions { WriteIndented = true });
        //File.WriteAllText(Path.Combine(directory, "seed.json"), json);
        return seedData;


    }

    [Test]
    public void CreateSeedJson()
    {
        
        var json = JsonSerializer.Serialize(CreateSeed(), new JsonSerializerOptions { WriteIndented = true });
        string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        File.WriteAllText(Path.Combine(directory, "seed.json"), json);
        
    }

    [Test]
    public void CheckOnDb()
    {
        var seedData = (SeedData)CreateSeed();
        var json = JsonSerializer.Serialize(seedData, new JsonSerializerOptions { WriteIndented = true });
        
          var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseSqlite("Data Source=ThesisManagement.db")
            .Options;

        using (var context = new ThesisDbContext(options))
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            try
            {
                context.Roles.AddRange(seedData.Roles);
                context.SaveChanges();
                Console.WriteLine($"Roles seeded successfully: {seedData.Roles.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding roles: {ex.Message}");
                throw;
            }

            try
            {
                context.BillingStatuses.AddRange(seedData.BillingStatuses);
                context.SaveChanges();
                Console.WriteLine($"Billing statuses seeded successfully: {seedData.BillingStatuses.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding billing statuses: {ex.Message}");
                throw;
            }

            try
            {
                context.ThesisStatuses.AddRange(seedData.ThesisStatuses);
                context.SaveChanges();
                Console.WriteLine($"Thesis statuses seeded successfully: {seedData.ThesisStatuses.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding thesis statuses: {ex.Message}");
                throw;
            }

            try
            {
                context.RequestTypes.AddRange(seedData.RequestTypes);
                context.SaveChanges();
                Console.WriteLine($"Request types seeded successfully: {seedData.RequestTypes.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding request types: {ex.Message}");
                throw;
            }

            try
            {
                context.RequestStatuses.AddRange(seedData.RequestStatuses);
                context.SaveChanges();
                Console.WriteLine($"Request statuses seeded successfully: {seedData.RequestStatuses.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding request statuses: {ex.Message}");
                throw;
            }

            try
            {
                context.ThesisOfferStatuses.AddRange(seedData.ThesisOfferStatuses);
                context.SaveChanges();
                Console.WriteLine($"Thesis offer statuses seeded successfully: {seedData.ThesisOfferStatuses.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding thesis offer statuses: {ex.Message}");
                throw;
            }

            try
            {
                context.SubjectAreas.AddRange(seedData.Topics);
                context.SaveChanges();
                Console.WriteLine($"Subject areas seeded successfully: {seedData.Topics.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding subject areas: {ex.Message}");
                throw;
            }

            try
            {
                context.Users.AddRange(seedData.Users);
                context.SaveChanges();
                Console.WriteLine($"Users seeded successfully: {seedData.Users.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding users: {ex.Message}");
                throw;
            }

            try
            {
                context.UserRoles.AddRange(seedData.UserRoles);
                context.SaveChanges();
                Console.WriteLine($"User roles seeded successfully: {seedData.UserRoles.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding user roles: {ex.Message}");
                throw;
            }

            try
            {
                context.UserToSubjectAreas.AddRange(seedData.UserTopics);
                context.SaveChanges();
                Console.WriteLine($"User to subject areas seeded successfully: {seedData.UserTopics.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding user to subject areas: {ex.Message}");
                throw;
            }

            try
            {
                context.Theses.AddRange(seedData.Theses);
                context.SaveChanges();
                Console.WriteLine($"Theses seeded successfully: {seedData.Theses.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding theses: {ex.Message}");
                throw;
            }

            try
            {
                context.ThesisDocuments.AddRange(seedData.ThesisDocuments);
                context.SaveChanges();
                Console.WriteLine($"Thesis documents seeded successfully: {seedData.ThesisDocuments.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding thesis documents: {ex.Message}");
                throw;
            }

            try
            {
                context.ThesisRequests.AddRange(seedData.ThesisRequests);
                context.SaveChanges();
                Console.WriteLine($"Thesis requests seeded successfully: {seedData.ThesisRequests.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding thesis requests: {ex.Message}");
                throw;
            }

            try
            {
                context.ThesisOffers.AddRange(seedData.ThesisOffers);
                context.SaveChanges();
                Console.WriteLine($"Thesis offers seeded successfully: {seedData.ThesisOffers.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding thesis offers: {ex.Message}");
                throw;
            }

            try
            {
                context.ThesisOfferApplications.AddRange(seedData.ThesisOfferApplications);
                context.SaveChanges();
                Console.WriteLine($"Thesis offer applications seeded successfully: {seedData.ThesisOfferApplications.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding thesis offer applications: {ex.Message}");
                throw;
            }

            Console.WriteLine("All seeding completed successfully!");
        }
    }

    public List<SubjectAreaDataAccessModel> TopicForSeeding()
    {
        return SeederSubjectArea.TopicForSeeding();

    }
    
    public static void SeedDatabase(ThesisDbContext context, SeedData seedData)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Seed in correct order to avoid FK violations
        context.Roles.AddRange(seedData.Roles);
        context.SaveChanges();

        context.BillingStatuses.AddRange(seedData.BillingStatuses);
        context.SaveChanges();

        context.ThesisStatuses.AddRange(seedData.ThesisStatuses);
        context.SaveChanges();

        context.RequestTypes.AddRange(seedData.RequestTypes);
        context.SaveChanges();

        context.RequestStatuses.AddRange(seedData.RequestStatuses);
        context.SaveChanges();

        context.ThesisOfferStatuses.AddRange(seedData.ThesisOfferStatuses);
        context.SaveChanges();

        context.SubjectAreas.AddRange(seedData.Topics);
        context.SaveChanges();

        context.Users.AddRange(seedData.Users);
        context.SaveChanges();

        context.UserRoles.AddRange(seedData.UserRoles);
        context.SaveChanges();

        context.UserToSubjectAreas.AddRange(seedData.UserTopics);
        context.SaveChanges();

        context.Theses.AddRange(seedData.Theses);
        context.SaveChanges();

        context.ThesisDocuments.AddRange(seedData.ThesisDocuments);
        context.SaveChanges();

        context.ThesisRequests.AddRange(seedData.ThesisRequests);
        context.SaveChanges();

        context.ThesisOffers.AddRange(seedData.ThesisOffers);
        context.SaveChanges();

        context.ThesisOfferApplications.AddRange(seedData.ThesisOfferApplications);
        context.SaveChanges();

        Console.WriteLine("Database seeded successfully!");
    }
}







