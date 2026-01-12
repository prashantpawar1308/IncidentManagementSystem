import React from 'react'
import GridScreen from './screens/GridScreen.jsx'
import CreateTicketScreen from './screens/CreateTicketScreen.jsx'

export const routes = [
  {
    path: '/grid',
    element: <GridScreen />,
    title: 'Grid'
  },
  {
    path: '/create-ticket',
    element: <CreateTicketScreen />,
    title: 'Create Ticket'
  }
]