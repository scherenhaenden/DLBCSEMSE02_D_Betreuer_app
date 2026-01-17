package com.example.betreuer_app.util;

import android.content.Context;

import com.example.betreuer_app.R;

public final class BillingStatusDisplayMapper {

    private BillingStatusDisplayMapper() {
    }

    public static String mapBillingStatusToDisplay(Context context, String billingStatus) {
        if (billingStatus == null || billingStatus.trim().isEmpty()) {
            return context.getString(R.string.billing_status_unknown);
        }

        switch (billingStatus) {
            case "NONE":
                return context.getString(R.string.billing_status_none);
            case "ISSUED":
                return context.getString(R.string.billing_status_issued);
            case "PAID":
                return context.getString(R.string.billing_status_paid);
            default:
                return context.getString(R.string.billing_status_unknown);
        }
    }
}
