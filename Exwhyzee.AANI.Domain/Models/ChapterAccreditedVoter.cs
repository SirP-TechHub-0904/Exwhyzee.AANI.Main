using Exwhyzee.AANI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exwhyzee.AANI.Domain.Models
{
    // updated existing model to include a unique one-time VoteToken and concurrency token
    public class ChapterAccreditedVoter
    {
        public long Id { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; } = default!;
        public DateTime Date { get; set; }

        public long? ChapterId { get; set; }
        public Chapter Chapter { get; set; } = default!;

        // state
        public bool Voted { get; set; }
        public DateTime? DateVoted { get; set; }

        // TOKEN METADATA (do not store plain token in DB)
        // store only a hashed representation (HMAC-SHA256 of token using server secret)

        public string? VoteTokenHash { get; set; }
        public DateTime? TokenCreatedAt { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        public DateTime? TokenSentAt { get; set; }

        // RowVersion for optimistic concurrency if needed
        public byte[]? RowVersion { get; set; }

        public long? ChapterElectionId { get; set; }
        public ChapterElection? ChapterElection { get; set; }
    }
    public class ElectionPosition
    {
        public long Id { get; set; }

        // The election this position belongs to
        public long ElectionId { get; set; }
        public ChapterElection Election { get; set; } = default!;

        // Human-readable name of the position (shown on the ballot)
        public string Name { get; set; } = string.Empty;

        // Optional short machine-friendly slug (useful for URLs or identifiers)
        public string? Slug { get; set; }

        // Optional longer description / guidance for voters
        public string? Description { get; set; }



        // Order in which this position appears on the ballot (smaller = earlier)
        public int BallotOrder { get; set; } = 0;

        // Whether this position is active (useful for hiding old positions while keeping history)
        public bool IsActive { get; set; } = true;

        // Audit/time fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Candidates for this position (ElectionCandidate should reference PositionId)
        public ICollection<ElectionCandidate> Candidates { get; set; } = new List<ElectionCandidate>();
    }
    // Link between an election and a contester/participant (candidate)
    public class ElectionCandidate
    {
        public long Id { get; set; }
        public long ElectionId { get; set; }
        public ChapterElection Election { get; set; } = default!;

        // Candidate/Contester ParticipantId
        public string CandidateParticipantId { get; set; } = string.Empty;
        public Participant CandidateParticipant { get; set; } = default!;

        public long? PositionId { get; set; }
        public ElectionPosition? Position { get; set; }

        public string? Manifesto { get; set; }
        public int BallotOrder { get; set; } // for ordering on the ballot
    }
    // New: an Election that is scoped to a Chapter
    public class ChapterElection
    {
        public long Id { get; set; }
        public long ChapterId { get; set; }
        public Chapter Chapter { get; set; } = default!;

        // e.g., "2026 Chapter Executive Election"
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public ElectionStatus ElectionStatus { get; set; } = ElectionStatus.Draft;

        public ICollection<ElectionCandidate> Candidates { get; set; } = new List<ElectionCandidate>();
    }



    // Vote record (audit-worthy): store minimal content or full choice list depending on privacy requirements
    public class Vote
    {
        public long Id { get; set; }

        public long ElectionId { get; set; }
        public ChapterElection Election { get; set; } = default!;

        // which chapter accredited voter / participant cast this ballot
        public long ChapterAccreditedVoterId { get; set; }
        public ChapterAccreditedVoter ChapterAccreditedVoter { get; set; } = default!;

        // When the vote was cast
        public DateTime CastAt { get; set; }

        // a server-generated receipt (GUID or hashed value) to let voter verify their vote was recorded
        public string ReceiptToken { get; set; } = Guid.NewGuid().ToString("N");

        // Choice records
        public ICollection<VoteChoice> Choices { get; set; } = new List<VoteChoice>();
    }

    // Individual choice (supports single-choice and ranked-choice if needed)
    public class VoteChoice
    {
        public long Id { get; set; }
        public long VoteId { get; set; }
        public Vote Vote { get; set; } = default!;

        public long CandidateId { get; set; }
        public ElectionCandidate Candidate { get; set; } = default!;

        // For single choice, Position = 1. For ranked ballots, use 1,2,3...
        public int Position { get; set; }
    }

    // Audit log (optional but recommended)
    public class VoteAudit
    {
        public long Id { get; set; }
        public long VoteId { get; set; }
        public Vote Vote { get; set; } = default!;
        public string Action { get; set; } = string.Empty; // e.g., "cast", "revoked"
        public DateTime Timestamp { get; set; }
        public string PerformedBy { get; set; } = string.Empty; // admin, system, or participant id
        public string? Notes { get; set; }
    }

    // Small, ephemeral server-side session that maps a validated token -> accredited voter + election.
    // Stored for a short time and referenced from an HttpOnly cookie (VoteSessionId).
    public class VoteSession
    {
        public long Id { get; set; }

        // Public session id stored in cookie (GUID N format). Unique index above.
        [Required]
        [MaxLength(64)]
        public string SessionId { get; set; } = Guid.NewGuid().ToString("N");

        // Which accredited voter this session represents
        public long AccreditedVoterId { get; set; }
        public ChapterAccreditedVoter AccreditedVoter { get; set; } = default!;

        // Election the session is for
        public long ElectionId { get; set; }
        public ChapterElection Election { get; set; } = default!;

        // Store the HMAC (hash) of the token that was used to create this session.
        // Use this to verify the session owner at submit time without exposing the plain token.
        [Required]
        [MaxLength(128)]
        public string TokenHash { get; set; } = string.Empty;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(30);
    }
    // NOTE: Participant and Chapter types are referenced above and expected in your domain already.
}