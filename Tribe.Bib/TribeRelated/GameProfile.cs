namespace Tribe.Bib.Models.CommunityManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GameProfile : EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string GameKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Genre { get; set; }

        [MaxLength(200)]
        public string? Platform { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? MetadataJson { get; set; }

        public string? DefaultRoleTemplateJson { get; set; }

        public string? DefaultSettingsJson { get; set; }

        public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();
    }
}
