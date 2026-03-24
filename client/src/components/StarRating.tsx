import { useState } from 'react';

interface StarRatingProps {
  /** Current average/display value (0–5 or null = no votes). */
  value: number | null;
  /** User's own rating for this idea (1–5 or undefined if not rated yet). */
  userValue?: number;
  /** If provided, stars become interactive buttons. */
  onChange?: (rating: number) => void;
}

function StarIcon({ filled }: { filled: boolean }) {
  return (
    <svg
      width="20"
      height="20"
      viewBox="0 0 24 24"
      fill={filled ? 'currentColor' : 'none'}
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2" />
    </svg>
  );
}

export function StarRating({ value, userValue, onChange }: StarRatingProps) {
  const [hover, setHover] = useState(0);

  // Displayed filled count: hover > user rating > average (rounded)
  const displayFilled = hover || userValue || Math.round(value ?? 0);

  return (
    <div className="flex items-center gap-0.5">
      {[1, 2, 3, 4, 5].map((star) => {
        const filled = star <= displayFilled;
        const isInteractive = !!onChange;

        return (
          <button
            key={star}
            type="button"
            disabled={!isInteractive}
            onClick={() => onChange?.(star)}
            onMouseEnter={() => isInteractive && setHover(star)}
            onMouseLeave={() => isInteractive && setHover(0)}
            /* Subtle variant: no bg, no border, 8px padding, rounded-full */
            className={[
              'rounded-full p-2 transition-colors',
              filled ? 'text-yellow-400' : 'text-[#D9D9D9]',
              isInteractive ? 'hover:text-yellow-300 cursor-pointer' : 'cursor-default',
            ].join(' ')}
            aria-label={`${star} star${star > 1 ? 's' : ''}`}
          >
            <StarIcon filled={filled} />
          </button>
        );
      })}
    </div>
  );
}
