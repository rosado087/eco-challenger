export interface ChallengeModel {
    id: number
    title: string
    description: string
    type: 'Daily' | 'Weekly'
    currentProgress: number
    maxProgress: number
    completed: boolean
    message: string
    success: boolean
    list: string[]
}