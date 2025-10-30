export type CollectionIconVariant =
  | "lighting"
  | "energy"
  | "security"
  | "generic";

export interface CollectionHighlight {
  name: string;
  description: string;
  query?: string;
  accent: string;
  icon: CollectionIconVariant;
}

export const featuredCollections: CollectionHighlight[] = [
  {
    name: "Pametna rasveta",
    description:
      "Ambijentalna i radna rasveta koja menja temperaturu i intenzitet prema vašem raspoloženju.",
    query: "rasveta",
    accent: "from-indigo-500/10 via-blue-500/10 to-sky-500/10",
    icon: "lighting",
  },
  {
    name: "Energetska efikasnost",
    description:
      "Termostati, senzori i analitika potrošnje koji optimizuju grejanje i hlađenje.",
    query: "energija",
    accent: "from-emerald-500/10 via-green-500/10 to-lime-500/10",
    icon: "energy",
  },
  {
    name: "Sigurnost doma",
    description:
      "Pametne brave, kamere i senzori pokreta koji štite prostor i šalju obaveštenja u realnom vremenu.",
    query: "sigurnost",
    accent: "from-amber-500/10 via-orange-500/10 to-rose-500/10",
    icon: "security",
  },
];
