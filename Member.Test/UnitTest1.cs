using System;
using System.IO;
using System.Linq;
using Member.Data;
using NUnit.Framework;

namespace Member.Test
{
    public class Tests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var dbName = "data.db";
            if (File.Exists(dbName)) File.Delete(dbName);
        }

        [TestCase(1, "Lukas", "Hirsch")]
        [TestCase(3, "Florian", "Hirsch")]
        [TestCase(5, "Isabelle", "Budßus")]
        public void AddGet1Member(int id, string surname, string name)
        {
            using (var db = new DatabaseContext())
            {
                var repository = new MemberRepository(db);

                // Hinzufügen zur Datenbank
                var myMember = new Data.Member {MemberId = id, Surname = surname, Name = name};
                repository.InsertMember(myMember);
                Assert.True(db.Members.Contains(myMember));

                // Holen aus Datenbank
                var myMemberGet = repository.GetMemberById(id);
                Assert.AreEqual(id, myMemberGet.MemberId);
                Assert.AreEqual(surname, myMemberGet.Surname);
                Assert.AreEqual(name, myMemberGet.Name);
            }
        }

        [Test]
        public void GetPsa()
        {
            using (var db = new DatabaseContext())
            {
                var id = 5;
                var memberRepository = new MemberRepository(db);
                var psaRepository = new PsaRepository(db);
                var member = memberRepository.GetMemberById(id);

                Assert.AreEqual(psaRepository.GetPsaById(id), psaRepository.GetPsaByMember(member));
            }
        }

        [Test]
        public void Add2MemberSameId()
        {
            Assert.Catch<InvalidOperationException>(delegate
            {
                using (var db = new DatabaseContext())
                {
                    var repository = new MemberRepository(db);

                    // Hinzufügen zur Datenbank
                    var myMember1 = new Data.Member {MemberId = 8, Surname = "Test", Name = "User"};
                    var myMember2 = new Data.Member {MemberId = 8, Surname = "Test", Name = "User"};
                    repository.InsertMember(myMember1);
                    repository.InsertMember(myMember2);
                }
            });
        }

        [Test]
        public void UpdateMember()
        {
            using (var db = new DatabaseContext())
            {
                var memberRepository = new MemberRepository(db);
                var unchangedname = memberRepository.GetMemberById(5).Name;
                var changedmember = memberRepository.GetMemberById(5);

                changedmember.Name = "Neugebauer";
                memberRepository.UpdateMember(changedmember);

                Assert.AreEqual(changedmember, memberRepository.GetMemberById(5));
                Assert.AreNotEqual(unchangedname, memberRepository.GetMemberById(5).Name);
            }
        }
    }
}