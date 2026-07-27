/**
 * Presentation helpers shared by the list/detail screens.
 *
 * Money note: `invoices.amount` is stored in cents (the seed data carries e.g.
 * 8545 for $85.45), while the `revenue` table is already in whole dollars.
 * `formatCents` and `formatDollars` keep that distinction explicit at the call
 * site instead of hiding a magic /100 somewhere in a template.
 */

const currency = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
});

const currencyCompact = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  maximumFractionDigits: 0,
});

export function formatCents(amount: number | string | null | undefined): string {
  const value = toNumber(amount);
  return value === null ? '—' : currency.format(value / 100);
}

export function formatDollars(amount: number | string | null | undefined): string {
  const value = toNumber(amount);
  return value === null ? '—' : currencyCompact.format(value);
}

export function formatDate(value: string | Date | null | undefined): string {
  if (!value) return '—';
  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return '—';
  return date.toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' });
}

export function initials(name: string | null | undefined): string {
  if (!name) return '?';
  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return '?';
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
  return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
}

/**
 * Deterministic avatar colour: the same person always gets the same swatch.
 * These are decorative fills behind white initials, not a data encoding — all
 * of them clear 4.5:1 against white text.
 */
const AVATAR_COLORS = [
  '#4f46e5', '#0f766e', '#b45309', '#9333ea',
  '#0369a1', '#be123c', '#15803d', '#7c3aed',
];

export function avatarColor(seed: string | null | undefined): string {
  if (!seed) return AVATAR_COLORS[0];
  let hash = 0;
  for (let i = 0; i < seed.length; i++) {
    hash = (hash * 31 + seed.charCodeAt(i)) >>> 0;
  }
  return AVATAR_COLORS[hash % AVATAR_COLORS.length];
}

const MONTH_ORDER = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

/** The revenue table comes back ordered by the DB, not by calendar month. */
export function byCalendarMonth<T extends { month: string }>(rows: T[]): T[] {
  return [...rows].sort(
    (a, b) => MONTH_ORDER.indexOf(a.month?.slice(0, 3)) - MONTH_ORDER.indexOf(b.month?.slice(0, 3)),
  );
}

function toNumber(value: number | string | null | undefined): number | null {
  if (value === null || value === undefined || value === '') return null;
  const parsed = typeof value === 'number' ? value : Number(value);
  return Number.isFinite(parsed) ? parsed : null;
}
