export interface ChallengeModel {
    id: number
    title: string
    description: string
    type: 'Daily' | 'Weekly'
    points: number
    maxProgress: number
  }  