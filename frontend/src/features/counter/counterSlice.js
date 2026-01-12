import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import api from '../../api/api'

export const fetchData = createAsyncThunk('counter/fetchData', async () => {
  const resp = await api.get('/todos/1')
  return resp.data
})

const counterSlice = createSlice({
  name: 'counter',
  initialState: { value: 0, status: 'idle', data: null },
  reducers: {
    increment: (state) => { state.value += 1 },
    decrement: (state) => { state.value -= 1 },
    incrementByAmount: (state, action) => { state.value += action.payload }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchData.pending, (state) => { state.status = 'loading' })
      .addCase(fetchData.fulfilled, (state, action) => { state.status = 'succeeded'; state.data = action.payload })
      .addCase(fetchData.rejected, (state) => { state.status = 'failed' })
  }
})

export const { increment, decrement, incrementByAmount } = counterSlice.actions
export default counterSlice.reducer
