using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gitfo;

internal class GitfoOptions
{
    private static GitfoOptions _default = default!;

    public const string OptionsFileName = ".gitfo";

    [JsonPropertyName("profiles")]
    public ProfileCollection Profiles { get; set; } = new();

    public class Profile
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("repos")]
        public List<RepositoryInfo> Repos { get; set; } = new();

        public Profile(string name, List<RepositoryInfo>? repos = null)
        {
            Name = name;
            Repos = repos ?? new List<RepositoryInfo>();
        }
    }

    [JsonConverter(typeof(ProfilesConverter))]
    public class ProfileCollection : IEnumerable<Profile>
    {
        private readonly Dictionary<string, Profile> _profiles = new();

        public Profile? this[string name]
        {
            get => _profiles.TryGetValue(name, out var profile) ? profile : null;
            set
            {
                if (value == null)
                    _profiles.Remove(name);
                else
                    _profiles[name] = value;
            }
        }

        public void Add(string name, List<RepositoryInfo> repos)
        {
            _profiles.Add(name, new Profile(name, repos));
        }

        public void Add(Profile profile)
        {
            _profiles.Add(profile.Name, profile);
        }

        public bool Remove(string name)
        {
            return _profiles.Remove(name);
        }

        public bool TryGetValue(string name, out Profile? profile)
        {
            return _profiles.TryGetValue(name, out profile);
        }

        public void Clear()
        {
            _profiles.Clear();
        }

        public IEnumerator<Profile> GetEnumerator()
        {
            return _profiles.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ProfilesConverter : JsonConverter<ProfileCollection>
    {
        public override ProfileCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var profiles = new ProfileCollection();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                string profileName = reader.GetString();
                reader.Read();

                var profileConfig = JsonSerializer.Deserialize<Profile>(ref reader, options);
                if (profileConfig != null)
                {
                    profileConfig.Name = profileName;
                    profiles.Add(profileConfig);
                }
            }

            return profiles;
        }

        public override void Write(Utf8JsonWriter writer, ProfileCollection value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var profile in value)
            {
                writer.WritePropertyName(profile.Name);
                JsonSerializer.Serialize(writer, profile, options);
            }

            writer.WriteEndObject();
        }
    }

    public class RepositoryInfo
    {
        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("repo")]
        public string RepoName { get; set; }

        [JsonPropertyName("defaultBranch")]
        public string? DefaultBranch { get; set; }

        [JsonPropertyName("pat")]
        public string? AuthToken { get; set; }

        [JsonPropertyName("folder")]
        public string? LocalFolder { get; set; }
    }

    public static GitfoOptions Default
    {
        get => _default ??= new GitfoOptions
        {
        };
    }

    internal static bool TryParse(string json, out GitfoOptions options)
    {
        try
        {
            var jsonOpts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var opts = JsonSerializer.Deserialize<GitfoOptions>(json, jsonOpts);
            if (opts != null)
            {
                options = opts;
                return true;
            }
        }
        catch
        {
            // NOP - will return default below
        }

        options = Default;
        return false;
    }
}