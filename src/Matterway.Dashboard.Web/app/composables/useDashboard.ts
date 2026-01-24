export const useDashboard = () => {
  const isNotificationsSlideoverOpen = useState('dashboard-notifications-open', () => false)

  return {
    isNotificationsSlideoverOpen
  }
}
