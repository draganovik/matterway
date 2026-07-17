export default defineAppConfig({
  ui: {
    colors: {
      primary: "orange",
      neutral: "stone",
    },
    card: {
      slots: {
        root: "border border-default shadow-sm !ring-0",
        header: "sm:px-4",
        body: "sm:p-4",
        footer: "sm:px-4",
      },
    },
    modal: {
      slots: {
        header: "min-h-14 sm:px-4",
        body: "sm:p-4",
        footer: "sm:px-4",
      },
    },
    dashboardNavbar: {
      slots: {
        root: "sm:px-4",
      },
    },
    dashboardPanel: {
      slots: {
        body: "sm:gap-4 sm:p-4",
      },
    },
  },
})
