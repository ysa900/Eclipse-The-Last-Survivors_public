using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class EnemyManager : Eclipse.Manager
    {
        //==================================================================
        // enemies Index
        // 0 : 미니언, 1 : 보스, 2 : 석상
        [ReadOnly] public List<List<Enemy>> enemies = new List<List<Enemy>>();
        public List<Enemy> debugForEnemies0 = new List<Enemy>();
        public List<Enemy> debugForEnemies1 = new List<Enemy>();
        public List<Enemy> debugForEnemies2 = new List<Enemy>();

        protected List<Enemy> nearestEnemies = new List<Enemy>();

        //==================================================================
        // Action 모음
        public Action allocateEnemy;

        //==================================================================
        private float meleeRange = 1f; // 플레이어 근접 우선 공격 사거리
        private float rangedRange = 3f; // 플레이어 공격 사거리

        //==================================================================

        private void Start()
        {
            allocateEnemy();
        }

        public void DebugEnemyList()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                for (int j = 0; j < enemies[i].Count; j++)
                {
                    Debug.Log("enemy : " + enemies[i][j]);
                }
            }
        }

        public void InitializeEnemies(int count)
        {
            enemies = new List<List<Enemy>>(count);
            for (int i = 0; i < count; i++)
            {
                enemies.Add(new List<Enemy>());
            }
        }

        // 가장 가까운 적 찾기(Enemy형)
        public Enemy FindNearestEnemy()
        {
            Enemy nearestEnemy = null;
            float minSquaredDistance = 100 * 100;

            Vector2 playerPosition = PlayerManager.player.transform.position;
            foreach (var enemyList in enemies)
            {
                foreach (var enemy in enemyList)
                {
                    if (enemy == null || nearestEnemies.Contains(enemy)) continue;

                    float sqrDistance = (playerPosition - (Vector2)enemy.transform.position).sqrMagnitude;
                    
                    if (sqrDistance < minSquaredDistance)
                    {
                        minSquaredDistance = sqrDistance;
                        nearestEnemy = enemy;
                    }
                }
            }

            if (nearestEnemy != null)
            {
                nearestEnemies.Add(nearestEnemy); // 찾은 적을 리스트에 추가
            }

            return nearestEnemy;
        }

        // 사거리 내 랜덤한 적에게 공격 시전(Enemy형)
        public Enemy GetRandomTargetInRange()
        {
            float minSquaredDistance = 100 * 100;
            bool foundMeleeTarget = false;
            List<Enemy> validEnemies = new List<Enemy>();

            Vector2 playerPosition = PlayerManager.player.transform.position;
            // 모든 적을 탐색하여 사거리 내 적만 필터링
            foreach (var enemyList in enemies)
            {
                foreach (var enemy in enemyList)
                {
                    if (enemy == null) continue;

                    float sqrDistance = (playerPosition - (Vector2)enemy.transform.position).sqrMagnitude;
                    
                    if (sqrDistance <= meleeRange * meleeRange)
                    {
                        // 근접 범위 내 적이 있으면 우선 선택
                        if (sqrDistance < minSquaredDistance)
                        {
                            minSquaredDistance = sqrDistance;
                            validEnemies.Add(enemy);
                            foundMeleeTarget = true;
                        }
                    }
                    else if (!foundMeleeTarget && sqrDistance <= rangedRange * rangedRange)
                    {
                        // 근접 적이 없을 때 원거리 적을 선택
                        if (sqrDistance < minSquaredDistance)
                        {
                            minSquaredDistance = sqrDistance;
                            validEnemies.Add(enemy);
                        }
                    }
                }
            }

            // 사거리 내 적이 없으면 null 반환
            if (validEnemies.Count == 0)
                return null;

            // 랜덤한 적 반환
            int randomIndex = UnityEngine.Random.Range(0, validEnemies.Count);
            return validEnemies[randomIndex];
        }

        // 스킬 사거리 내 Enemy 필터링하는 함수(Enemy 리스트)
        public List<Enemy> GetEnemiesInRange(float skillRange)
        {
            List<Enemy> enemiesInRange = new List<Enemy>();
            Vector2 playerPosition = PlayerManager.player.transform.position;

            foreach (var enemyList in enemies)
            {
                foreach (var enemy in enemyList)
                {
                    if (enemy == null) continue;

                    float sqrDistance = (playerPosition - (Vector2)enemy.transform.position).sqrMagnitude;
                    if (sqrDistance <= skillRange * skillRange) // 스킬 사거리 내 적 필터링
                    {
                        enemiesInRange.Add(enemy);
                    }
                }
            }

            return enemiesInRange;
        }

        public Enemy FindMostClusteredEnemy(float skillRange, float cellSize = 1f)
        {
            List<Enemy> enemiesInRange = GetEnemiesInRange(skillRange);
            if (enemiesInRange.Count == 0) return null;

            // Step 1: 셀에 적들을 배치 (셀 좌표를 키로)
            Dictionary<Vector2Int, List<Enemy>> grid = new Dictionary<Vector2Int, List<Enemy>>();

            foreach (var enemy in enemiesInRange)
            {
                Vector2 pos = enemy.transform.position;
                Vector2Int cell = new Vector2Int(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.y / cellSize));

                if (!grid.ContainsKey(cell))
                    grid[cell] = new List<Enemy>();

                grid[cell].Add(enemy);
            }

            // Step 2: 가장 많은 적이 모여 있는 셀 찾기
            var densestCell = grid.OrderByDescending(kv => kv.Value.Count).First();
            var clusterEnemies = densestCell.Value;

            // Step 3: 그 셀의 평균 위치 계산
            Vector2 center = Vector2.zero;
            foreach (var enemy in clusterEnemies)
                center += (Vector2)enemy.transform.position;
            center /= clusterEnemies.Count;

            // Step 4: 중심에 가장 가까운 적 반환
            Enemy closest = null;
            float closestDist = float.MaxValue;

            foreach (var enemy in clusterEnemies)
            {
                float dist = ((Vector2)enemy.transform.position - center).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = enemy;
                }
            }

            return closest;
        }

        public void CalculateEnemiesCounts()
        {
            // 미니언 enemies 리스트에 받아오기
            allocateEnemy();

            // 보스, 석상은 이미 받아져 있는 상태
        }

        public void ClearNearestEnemies()
        {
            nearestEnemies.Clear();
        }
    }

}
