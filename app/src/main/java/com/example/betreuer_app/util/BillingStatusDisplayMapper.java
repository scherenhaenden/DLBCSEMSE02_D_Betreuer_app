package com.example.betreuer_app.util;

import android.content.Context;

import com.example.betreuer_app.R;

public final class BillingStatusDisplayMapper {

    private BillingStatusDisplayMapper() {
    }

    /**
     * Maps the billing status to a corresponding display string.
     *
     * This method checks if the billingStatus is null or empty and returns a default unknown status if so.
     * It then uses a switch statement to return the appropriate string resource based on the value of billingStatus,
     * handling cases for "NONE", "ISSUED", and "PAID". If the billingStatus does not match any known cases,
     * it defaults to the unknown status.
     *
     * @param context the context used to access string resources
     * @param billingStatus the billing status to be mapped to a display string
     */
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
