import { ChallengeModel } from './challenge-model'

export interface ChallengeListModel {
    success: boolean
    message: string
    dailyChallenges: { challenge: ChallengeModel; wasConcluded: boolean }[]
    weeklyChallenges: {
        challenge: ChallengeModel
        progress: number
        wasConcluded: boolean
    }[]
}
