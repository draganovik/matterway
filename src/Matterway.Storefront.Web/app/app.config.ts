export default defineAppConfig({
  ui: {
    colors: {
      primary: "cyan",
      neutral: "stone",
    },
    card: {
      slots: {
        root: "border border-default !bg-elevated shadow-sm !ring-0",
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
  },
})
