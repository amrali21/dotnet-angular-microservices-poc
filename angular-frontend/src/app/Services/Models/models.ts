export type LatestInvoice = {
    id: string;
    name: string;
    image_url: string;
    email: string;
    /** Stored in cents — see Shared/display.ts formatCents. */
    amount: string;
    status: string;
    date?: string;
};

export type Customer = {
    id: string;
    name: string;
    email: string;
    image_url: string;
};

export type SearchFilter = {
    pageSize: number;
    pageIndex: number;
    // length: number;
}

export type CustomerFiler = {
    pageSize: number;
    pageIndex: number;

    query: string;
}

export type InvoiceFilter = {
    pageSize: number;
    pageIndex: number;

    query: string;
}


export type PagedResult = {
    data: any[];
    count: number;
}

export type RevenuePoint = {
    month: string;
    revenue: number;
}

export type KpiCardData = {
    totalBilled: number;
    collected: number;
    outstanding: number;
    customers: number;
}

export type AuthUser = {
    id: string;
    email: string;
    displayName: string;
};

export type RegisterRequest = {
    name: string;
    email: string;
    password: string;
};

export type LoginRequest = {
    email: string;
    password: string;
};

export type AuthResponse = {
    token: string;
    expiresAtUtc: string;
    user: AuthUser;
};