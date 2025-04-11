export type ChallengeType = 'Daily' | 'Weekly'

export interface ChallengeModel {
    id: number
    title: string
    description: string
    type: ChallengeType
    points: number
    maxProgress: number
}
