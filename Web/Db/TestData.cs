using Web.Enums;
using Web.Models;

namespace Web.Db
{
    public class TestData
    {
        private readonly Random _random = new();

        private List<PostCategory> posts = new List<PostCategory>();

        #region Random generation
        private static readonly Category[] categories =
        {
            new Category { Title = "Developing" },
            new Category { Title = "Releases" },
            new Category { Title = "Other" }
        };

        /// <summary>Real-world artist + album + genre for seed data (25 per style).</summary>
        private readonly record struct DiscographyEntry(string ArtistName, string AlbumTitle, string GenreName);

        private static IReadOnlyList<DiscographyEntry> BuildDiscographyCatalog()
        {
            const string Rock = "Rock";
            const string HeavyMetal = "Heavy Metal";
            const string Jazz = "Jazz";
            const string Pop = "Pop";
            const string Industrial = "Industrial";

            return
            [
                // Rock (25)
                new("The Beatles", "Abbey Road", Rock),
                new("Led Zeppelin", "Led Zeppelin IV", Rock),
                new("Pink Floyd", "The Dark Side of the Moon", Rock),
                new("Queen", "A Night at the Opera", Rock),
                new("Fleetwood Mac", "Rumours", Rock),
                new("The Rolling Stones", "Sticky Fingers", Rock),
                new("Bruce Springsteen", "Born to Run", Rock),
                new("Tom Petty and the Heartbreakers", "Full Moon Fever", Rock),
                new("Dire Straits", "Brothers in Arms", Rock),
                new("Eagles", "Hotel California", Rock),
                new("U2", "The Joshua Tree", Rock),
                new("R.E.M.", "Automatic for the People", Rock),
                new("Pearl Jam", "Ten", Rock),
                new("Nirvana", "Nevermind", Rock),
                new("Radiohead", "OK Computer", Rock),
                new("Foo Fighters", "The Colour and the Shape", Rock),
                new("Arctic Monkeys", "AM", Rock),
                new("The Strokes", "Is This It", Rock),
                new("Oasis", "(What's the Story) Morning Glory?", Rock),
                new("Tom Waits", "Rain Dogs", Rock),
                new("David Bowie", "The Rise and Fall of Ziggy Stardust and the Spiders from Mars", Rock),
                new("The Who", "Who's Next", Rock),
                new("Creedence Clearwater Revival", "Cosmo's Factory", Rock),
                new("The Jimi Hendrix Experience", "Are You Experienced", Rock),
                new("Bob Dylan", "Highway 61 Revisited", Rock),

                // Heavy Metal (25)
                new("Metallica", "Master of Puppets", HeavyMetal),
                new("Iron Maiden", "The Number of the Beast", HeavyMetal),
                new("Black Sabbath", "Paranoid", HeavyMetal),
                new("Judas Priest", "British Steel", HeavyMetal),
                new("Slayer", "Reign in Blood", HeavyMetal),
                new("Megadeth", "Rust in Peace", HeavyMetal),
                new("Pantera", "Cowboys from Hell", HeavyMetal),
                new("Tool", "Ænima", HeavyMetal),
                new("Opeth", "Blackwater Park", HeavyMetal),
                new("Dream Theater", "Images and Words", HeavyMetal),
                new("System of a Down", "Toxicity", HeavyMetal),
                new("Slipknot", "Iowa", HeavyMetal),
                new("Anthrax", "Among the Living", HeavyMetal),
                new("Motörhead", "Ace of Spades", HeavyMetal),
                new("Ozzy Osbourne", "Blizzard of Ozz", HeavyMetal),
                new("Dio", "Holy Diver", HeavyMetal),
                new("Meshuggah", "Destroy Erase Improve", HeavyMetal),
                new("Gojira", "From Mars to Sirius", HeavyMetal),
                new("Mastodon", "Leviathan", HeavyMetal),
                new("Lamb of God", "As the Palaces Burn", HeavyMetal),
                new("Sepultura", "Roots", HeavyMetal),
                new("Cannibal Corpse", "Tomb of the Mutilated", HeavyMetal),
                new("Death", "Human", HeavyMetal),
                new("Amon Amarth", "Twilight of the Thunder God", HeavyMetal),
                new("Avenged Sevenfold", "City of Evil", HeavyMetal),

                // Jazz (25)
                new("Miles Davis", "Kind of Blue", Jazz),
                new("John Coltrane", "A Love Supreme", Jazz),
                new("The Dave Brubeck Quartet", "Time Out", Jazz),
                new("Duke Ellington", "Ellington at Newport", Jazz),
                new("Louis Armstrong", "Hot Fives and Sevens", Jazz),
                new("Bill Evans Trio", "Waltz for Debby", Jazz),
                new("Charles Mingus", "Mingus Ah Um", Jazz),
                new("Thelonious Monk", "Brilliant Corners", Jazz),
                new("Herbie Hancock", "Head Hunters", Jazz),
                new("Weather Report", "Heavy Weather", Jazz),
                new("Pat Metheny", "Bright Size Life", Jazz),
                new("Keith Jarrett", "The Köln Concert", Jazz),
                new("Oscar Peterson Trio", "Night Train", Jazz),
                new("Nina Simone", "Little Girl Blue", Jazz),
                new("Ella Fitzgerald", "Ella in Berlin", Jazz),
                new("Chet Baker", "Chet Baker Sings", Jazz),
                new("Stan Getz", "Getz/Gilberto", Jazz),
                new("Wayne Shorter", "Speak No Evil", Jazz),
                new("Art Blakey & The Jazz Messengers", "Moanin'", Jazz),
                new("Sonny Rollins", "Saxophone Colossus", Jazz),
                new("Return to Forever", "Romantic Warrior", Jazz),
                new("Jaco Pastorius", "Jaco Pastorius", Jazz),
                new("Esbjörn Svensson Trio", "Vi är inte längre där", Jazz),
                new("Brad Mehldau", "The Art of the Trio Vol. 1", Jazz),
                new("Kamasi Washington", "The Epic", Jazz),

                // Pop (25)
                new("Michael Jackson", "Thriller", Pop),
                new("Madonna", "Like a Prayer", Pop),
                new("Prince and The Revolution", "Purple Rain", Pop),
                new("Whitney Houston", "Whitney", Pop),
                new("George Michael", "Faith", Pop),
                new("Adele", "21", Pop),
                new("Taylor Swift", "1989", Pop),
                new("Beyoncé", "Lemonade", Pop),
                new("Lady Gaga", "The Fame Monster", Pop),
                new("Britney Spears", "...Baby One More Time", Pop),
                new("Christina Aguilera", "Stripped", Pop),
                new("Justin Timberlake", "FutureSex/LoveSounds", Pop),
                new("Bruno Mars", "24K Magic", Pop),
                new("Ed Sheeran", "÷ (Divide)", Pop),
                new("Dua Lipa", "Future Nostalgia", Pop),
                new("The Weeknd", "After Hours", Pop),
                new("Billie Eilish", "When We All Fall Asleep, Where Do We Go?", Pop),
                new("Robyn", "Body Talk", Pop),
                new("Kylie Minogue", "Fever", Pop),
                new("ABBA", "Arrival", Pop),
                new("Spice Girls", "Spice", Pop),
                new("Katy Perry", "Teenage Dream", Pop),
                new("Rihanna", "Good Girl Gone Bad", Pop),
                new("Ariana Grande", "Thank U, Next", Pop),
                new("Harry Styles", "Harry's House", Pop),

                // Industrial (25)
                new("Nine Inch Nails", "The Downward Spiral", Industrial),
                new("Ministry", "Psalm 69", Industrial),
                new("Skinny Puppy", "Too Dark Park", Industrial),
                new("Front Line Assembly", "Tactical Neural Implant", Industrial),
                new("KMFDM", "Nihil", Industrial),
                new("Rammstein", "Sehnsucht", Industrial),
                new("Combichrist", "This Is Where Death Begins", Industrial),
                new("Marilyn Manson", "Antichrist Superstar", Industrial),
                new("Rob Zombie", "Hellbilly Deluxe", Industrial),
                new("Throbbing Gristle", "20 Jazz Funk Greats", Industrial),
                new("Einstürzende Neubauten", "Halber Mensch", Industrial),
                new("Coil", "Horse Rotorvator", Industrial),
                new("Cabaret Voltaire", "Red Mecca", Industrial),
                new("Front 242", "Tyranny >For You<", Industrial),
                new("Nitzer Ebb", "That Total Age", Industrial),
                new("Laibach", "Opus Dei", Industrial),
                new("And One", "I.S.T.", Industrial),
                new("Wumpscut", "Bunkertor 7", Industrial),
                new("Die Krupps", "I", Industrial),
                new("Feindflug", "Feindflug", Industrial),
                new("3TEETH", "<shutdown.exe>", Industrial),
                new("Author & Punisher", "Melk En Honing", Industrial),
                new("Youth Code", "Commitment to Complications", Industrial),
                new("Cubanate", "Interference", Industrial),
                new("Pigface", "Gub", Industrial),
            ];
        }

        private static readonly string[] SeedCountryNames =
        {
            "USA", "UK", "Germany", "France", "Japan", "Sweden", "Norway", "Finland",
            "Poland", "Italy", "Spain", "Canada", "Australia", "Brazil", "Argentina",
            "Netherlands", "Belgium", "Austria", "Switzerland", "Czech Republic"
        };

        static string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static DateTime GenerateRandomDate(DateTime start, DateTime end)
        {
            var random = new Random();
            var range = (end - start).Days;
            return start.AddDays(random.Next(range));
        }

        static Category GetRandomCategory()
        {
            var random = new Random();
            int index = random.Next(categories.Length);
            return categories[index];
        }
        static bool GetRandomBool()
        {
            var random = new Random();
            return random.Next(2) == 1;
        }
        #endregion

        public List<PostCategory> GetPosts()
        {
            var now = TimeProvider.System.GetLocalNow().LocalDateTime;
            for (int i = 1; i <= 100; i++)
            {
                var post = new Post()
                {
                    Content = GenerateRandomText(1024),
                    Description = GenerateRandomText(50),
                    Title = GenerateRandomText(32),
                    CreatedDate = GenerateRandomDate(new DateTime(2015, 1, 1), DateTime.Today),
                    UpdatedDate = now,
                    IsDraft = GetRandomBool(),
                };

                var postCategory = new PostCategory
                {
                    Post = post,
                    Category = GetRandomCategory()
                };

                posts.Add(postCategory);
            }

            return posts;
        }

        /// <summary>DSD formats from <see cref="Context.OnModelCreating"/> — IDs 2–5.</summary>
        private static bool IsDsdDigitalFormatId(int digitalFormatId) => digitalFormatId is >= 2 and <= 5;

        private FormatInfo CreateRandomFormatInfo()
        {
            int digitalFormatId = _random.Next(1, 7);

            int bitnessId;
            int samplingId;

            if (IsDsdDigitalFormatId(digitalFormatId))
            {
                bitnessId = 1;
                int[] dsdSamplingIds = [4, 5, 6, 7];
                samplingId = dsdSamplingIds[_random.Next(dsdSamplingIds.Length)];
            }
            else
            {
                int[] pcmBitnessIds = [2, 3];
                int[] pcmSamplingIds = [1, 2, 3];
                bitnessId = pcmBitnessIds[_random.Next(pcmBitnessIds.Length)];
                samplingId = pcmSamplingIds[_random.Next(pcmSamplingIds.Length)];
            }

            return new FormatInfo
            {
                BitnessId = bitnessId,
                DigitalFormatId = digitalFormatId,
                SourceFormatId = _random.Next(1, 7),
                SamplingId = samplingId,
                VinylStateId = _random.Next(1, 7),
            };
        }

        private EquipmentInfo CreateRandomEquipmentInfo(
            IReadOnlyList<Adc> adcs,
            IReadOnlyList<Amplifier> amplifiers,
            IReadOnlyList<Cartridge> cartridges,
            IReadOnlyList<Player> players,
            IReadOnlyList<Wire> wires)
        {
            return new EquipmentInfo
            {
                Adc = adcs[_random.Next(adcs.Count)],
                Amplifier = amplifiers[_random.Next(amplifiers.Count)],
                Cartridge = cartridges[_random.Next(cartridges.Count)],
                Player = players[_random.Next(players.Count)],
                Wire = wires[_random.Next(wires.Count)],
            };
        }

        private Digitization CreateDigitization(
            IReadOnlyList<Year> years,
            IReadOnlyList<Reissue> reissues,
            IReadOnlyList<Country> countries,
            IReadOnlyList<Label> labels,
            IReadOnlyList<Storage> storages,
            IReadOnlyList<Adc> adcs,
            IReadOnlyList<Amplifier> amplifiers,
            IReadOnlyList<Cartridge> cartridges,
            IReadOnlyList<Player> players,
            IReadOnlyList<Wire> wires)
        {
            Year? year = _random.Next(4) == 0 ? null : years[_random.Next(years.Count)];
            Reissue? reissue = _random.Next(3) == 0 ? null : reissues[_random.Next(reissues.Count)];
            Country? country = _random.Next(4) == 0 ? null : countries[_random.Next(countries.Count)];
            Label? label = _random.Next(4) == 0 ? null : labels[_random.Next(labels.Count)];
            Storage? storage = _random.Next(4) == 0 ? null : storages[_random.Next(storages.Count)];

            return new Digitization
            {
                AddedDate = TimeProvider.System.GetLocalNow().LocalDateTime,
                Source = "https://somelink.com",
                Discogs = "https://somelink.com",
                Size = Math.Round(_random.NextDouble() * 8 + 0.5, 2),
                Year = year,
                Reissue = reissue,
                Country = country,
                Label = label,
                Storage = storage,
                FormatInfo = CreateRandomFormatInfo(),
                EquipmentInfo = CreateRandomEquipmentInfo(adcs, amplifiers, cartridges, players, wires),
            };
        }

        /// <summary>
        /// <see cref="Manufacturer.Name"/> is globally unique in seed data.
        /// </summary>
        private static Manufacturer Mfr(string name) =>
            new() { Name = name };

        private static void AddAdc(List<Adc> list, Dictionary<string, Manufacturer> reg, string mfrName, string modelName)
        {
            if (!reg.TryGetValue(mfrName, out var m))
            {
                m = Mfr(mfrName);
                reg[mfrName] = m;
            }
            list.Add(new Adc { Name = modelName, Manufacturer = m });
        }

        private static void AddAmplifier(List<Amplifier> list, Dictionary<string, Manufacturer> reg, string mfrName, string modelName)
        {
            if (!reg.TryGetValue(mfrName, out var m))
            {
                m = Mfr(mfrName);
                reg[mfrName] = m;
            }
            list.Add(new Amplifier { Name = modelName, Manufacturer = m });
        }

        private static void AddCartridge(List<Cartridge> list, Dictionary<string, Manufacturer> reg, string mfrName, string modelName)
        {
            if (!reg.TryGetValue(mfrName, out var m))
            {
                m = Mfr(mfrName);
                reg[mfrName] = m;
            }
            list.Add(new Cartridge { Name = modelName, Manufacturer = m });
        }

        private static void AddPlayer(List<Player> list, Dictionary<string, Manufacturer> reg, string mfrName, string modelName)
        {
            if (!reg.TryGetValue(mfrName, out var m))
            {
                m = Mfr(mfrName);
                reg[mfrName] = m;
            }
            list.Add(new Player { Name = modelName, Manufacturer = m });
        }

        private static void AddWire(List<Wire> list, Dictionary<string, Manufacturer> reg, string mfrName, string modelName)
        {
            if (!reg.TryGetValue(mfrName, out var m))
            {
                m = Mfr(mfrName);
                reg[mfrName] = m;
            }
            list.Add(new Wire { Name = modelName, Manufacturer = m });
        }

        private static List<Adc> BuildRealWorldAdcs()
        {
            var list = new List<Adc>();
            var reg = new Dictionary<string, Manufacturer>(StringComparer.Ordinal);
            // Interfaces / recorders with well-known ADC stages (models from manufacturer lineups)
            AddAdc(list, reg, "RME", "Fireface UCX II");
            AddAdc(list, reg, "RME", "Fireface UFX III");
            AddAdc(list, reg, "MOTU", "828 (2024)");
            AddAdc(list, reg, "MOTU", "896mk3 Hybrid");
            AddAdc(list, reg, "TASCAM", "US-16x08");
            AddAdc(list, reg, "TASCAM", "Model 12");
            AddAdc(list, reg, "Focusrite", "Scarlett 18i20 3rd Gen");
            AddAdc(list, reg, "Universal Audio", "Apollo x8");
            AddAdc(list, reg, "Steinberg", "UR-RT4");
            AddAdc(list, reg, "Sony Professional", "PCM-D100");
            AddAdc(list, reg, "Apogee Electronics", "Symphony Desktop");
            AddAdc(list, reg, "PreSonus", "Quantum 2626");
            AddAdc(list, reg, "Audient", "iD44 MKII");
            AddAdc(list, reg, "Antelope Audio", "Discrete 8 Pro Synergy Core");
            AddAdc(list, reg, "Solid State Logic", "SSL 12");
            AddAdc(list, reg, "Zoom", "F6");
            return list;
        }

        private static List<Amplifier> BuildRealWorldAmplifiers()
        {
            var list = new List<Amplifier>();
            var reg = new Dictionary<string, Manufacturer>(StringComparer.Ordinal);
            AddAmplifier(list, reg, "Parasound", "JC3+");
            AddAmplifier(list, reg, "Parasound", "Zphono XRM");
            AddAmplifier(list, reg, "Rega Electronics", "Aura MC");
            AddAmplifier(list, reg, "Rega Electronics", "Aos MC");
            AddAmplifier(list, reg, "Rotel", "A14 MKII");
            AddAmplifier(list, reg, "Yamaha", "A-S1200");
            AddAmplifier(list, reg, "Cambridge Audio", "Alva Duo");
            AddAmplifier(list, reg, "Pro-Ject Box Design", "Phono Box RS2");
            AddAmplifier(list, reg, "Schiit Audio", "Mani 2");
            AddAmplifier(list, reg, "iFi audio", "Zen Phono");
            AddAmplifier(list, reg, "Musical Fidelity", "M6x Vinyl");
            AddAmplifier(list, reg, "McIntosh", "MP100");
            AddAmplifier(list, reg, "Vincent Audio", "PHO-701");
            AddAmplifier(list, reg, "NAD", "PP 2e");
            return list;
        }

        private static List<Cartridge> BuildRealWorldCartridges()
        {
            var list = new List<Cartridge>();
            var reg = new Dictionary<string, Manufacturer>(StringComparer.Ordinal);
            AddCartridge(list, reg, "Ortofon", "MC X40");
            AddCartridge(list, reg, "Ortofon", "MC X20");
            AddCartridge(list, reg, "Ortofon", "2M Black LVB 250");
            AddCartridge(list, reg, "Audio-Technica Pickups", "AT-OC9XSL");
            AddCartridge(list, reg, "Audio-Technica Pickups", "AT-VM95E");
            AddCartridge(list, reg, "Denon Phono", "DL-103");
            AddCartridge(list, reg, "Sumiko", "Moonstone");
            AddCartridge(list, reg, "Grado Labs", "Prestige Gold3");
            AddCartridge(list, reg, "Nagaoka", "MP-200");
            AddCartridge(list, reg, "Hana", "EH");
            AddCartridge(list, reg, "Goldring", "E3");
            AddCartridge(list, reg, "Shure", "M44-7");
            AddCartridge(list, reg, "Lyra", "Delos");
            return list;
        }

        private static List<Player> BuildRealWorldPlayers()
        {
            var list = new List<Player>();
            var reg = new Dictionary<string, Manufacturer>(StringComparer.Ordinal);
            AddPlayer(list, reg, "Technics", "SL-1200GR");
            AddPlayer(list, reg, "Technics", "SL-1210MK7");
            AddPlayer(list, reg, "Sony Home Audio", "PS-HX500");
            AddPlayer(list, reg, "Pro-Ject Turntables", "Debut Carbon EVO");
            AddPlayer(list, reg, "Rega Turntables", "Planar 3");
            AddPlayer(list, reg, "Denon Turntables", "DP-3000NE");
            AddPlayer(list, reg, "Audio-Technica Turntables", "AT-LP140XP");
            AddPlayer(list, reg, "Marantz", "TT-15S1");
            AddPlayer(list, reg, "Clearaudio", "Concept Black");
            AddPlayer(list, reg, "Yamaha Turntables", "TT-N503");
            AddPlayer(list, reg, "Fluance", "RT85");
            AddPlayer(list, reg, "U-Turn Audio", "Orbit Theory");
            return list;
        }

        private static List<Wire> BuildRealWorldWires()
        {
            var list = new List<Wire>();
            var reg = new Dictionary<string, Manufacturer>(StringComparer.Ordinal);
            AddWire(list, reg, "AudioQuest", "Big Sur RCA");
            AddWire(list, reg, "AudioQuest", "Tower RCA");
            AddWire(list, reg, "Nordost", "White Lightning RCA");
            AddWire(list, reg, "Nordost", "Valhalla 2 RCA");
            AddWire(list, reg, "Wireworld", "Oasis 10 RCA");
            AddWire(list, reg, "Wireworld", "Silver Eclipse 10 RCA");
            AddWire(list, reg, "CHORD", "C-line RCA");
            AddWire(list, reg, "Kimber Kable", "Timbre RCA");
            AddWire(list, reg, "Supra", "Sword-ISL RCA");
            AddWire(list, reg, "QED", "Performance Audio 40 RCA");
            AddWire(list, reg, "Van den Hul", "D-102 III Hybrid RCA");
            AddWire(list, reg, "Monster Cable", "Interlink 400 MKII");
            AddWire(list, reg, "LG", "OFC Speaker Cable 2x1.5mm");
            AddWire(list, reg, "Furutech", "Lineflux RCA");
            AddWire(list, reg, "Sony", "MUC-B20SM1");
            return list;
        }

        public List<Album> GetAlbums()
        {
            var result = new List<Album>();

            var catalog = BuildDiscographyCatalog();
            var genreByName = new Dictionary<string, Genre>(StringComparer.Ordinal)
            {
                ["Rock"] = new Genre { Name = "Rock" },
                ["Heavy Metal"] = new Genre { Name = "Heavy Metal" },
                ["Jazz"] = new Genre { Name = "Jazz" },
                ["Pop"] = new Genre { Name = "Pop" },
                ["Industrial"] = new Genre { Name = "Industrial" },
            };

            var years = Enumerable.Range(1950, 76).Select(y => new Year { Value = y }).ToList();
            var reissues = Enumerable.Range(1975, 51).Select(y => new Reissue { Value = y }).ToList();

            var countries = SeedCountryNames.Select(name => new Country { Name = name }).ToList();

            var labels = Enumerable.Range(1, 45)
                .Select(i => new Label { Name = $"Seed Label {i:D3}" })
                .ToList();

            var storages = Enumerable.Range(1, 24)
                .Select(i => new Storage { Name = $"Seed Storage {i:D2}" })
                .ToList();

            var adcs = BuildRealWorldAdcs();
            var amplifiers = BuildRealWorldAmplifiers();
            var cartridges = BuildRealWorldCartridges();
            var players = BuildRealWorldPlayers();
            var wires = BuildRealWorldWires();

            foreach (var entry in catalog)
            {
                int digitizationCount = _random.Next(0, 101);
                var digitizations = new List<Digitization>(digitizationCount);
                for (int d = 0; d < digitizationCount; d++)
                {
                    digitizations.Add(CreateDigitization(
                        years, reissues, countries, labels, storages,
                        adcs, amplifiers, cartridges, players, wires));
                }

                result.Add(new Album
                {
                    Title = entry.AlbumTitle,
                    Artist = new Artist { Name = entry.ArtistName },
                    Genre = genreByName[entry.GenreName],
                    Digitizations = digitizations,
                });
            }

            return result;
        }
    }
}
