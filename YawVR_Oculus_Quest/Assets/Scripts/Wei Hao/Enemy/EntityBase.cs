using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    //// List of Fixed entities
    //List<GameObject> entityList = new List<GameObject>();

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    ////// Update is called once per frame
    ////void Update()
    ////{

    ////}
    
    //// Update all entities
    //void Update(double _dt)
    //{
    //    // Update all entities

    //    std::list<EntityBase*>::iterator it, end;
    //    end = entityList.end();
    //    for (it = entityList.Count(); it != end; ++it)
    //    {
    //        (*it)->Update(_dt);
    //    }

    //    // Check for Collision amongst entities with collider properties
    //    CheckForCollision();

    //    // Clean up entities that are done
    //    it = entityList.begin();
    //    while (it != end)
    //    {
    //        if ((*it)->IsDone())
    //        {
    //            // Delete if done
    //            delete* it;
    //            it = entityList.erase(it);
    //        }
    //        else
    //        {
    //            // Move on otherwise
    //            ++it;
    //        }
    //    }

    //    // Clean up projectiles that are done
    //    item_projectile = projectileList.begin();
    //    while (item_projectile != projectileList.end())
    //    {
    //        if ((*item_projectile)->IsDone())
    //        {
    //            cout << "Removing a projectile..." << endl;
    //            // Delete if done
    //            delete* item_projectile;
    //            projectileList.erase(item_projectile++);
    //        }
    //        else
    //        {
    //            // Move on otherwise
    //            ++item_projectile;
    //        }
    //    }
    //}

    //// Render all entities
    //void Render()
    //{
    //    // Render all entities
    //    std::list<EntityBase*>::iterator it, end;
    //    end = entityList.end();
    //    for (it = entityList.begin(); it != end; ++it)
    //    {
    //        (*it)->Render();
    //    }

    //    // Render the projectiles
    //    std::list<EntityBase*>::iterator item_projectile = projectileList.begin();
    //    while (item_projectile != projectileList.end())
    //    {
    //        (*item_projectile)->Render();
    //        ++item_projectile;
    //    }
    //}

    //// Render the UI entities
    //void RenderUI()
    //{
    //    // Render all entities UI
    //    std::list<EntityBase*>::iterator it, end;
    //    end = entityList.end();
    //    for (it = entityList.begin(); it != end; ++it)
    //    {
    //        (*it)->RenderUI();
    //    }
    //}

    //// Add an entity to this EntityManager
    //void AddEntity(EntityBase* _newEntity, ENTITY_TYPE sEntityType)
    //{
    //    if (sEntityType == FIXED)
    //        entityList.push_back(_newEntity);
    //    else if (sEntityType == PROJECTILE)
    //        projectileList.push_back(_newEntity);
    //}

    //// Remove an entity from this EntityManager
    //bool RemoveEntity(EntityBase* _existingEntity)
    //{
    //    // Find the entity's iterator
    //    std::list<EntityBase*>::iterator findIter = std::find(entityList.begin(), entityList.end(), _existingEntity);

    //    // Delete the entity if found
    //    if (findIter != entityList.end())
    //    {
    //        delete* findIter;
    //        findIter = entityList.erase(findIter);
    //        return true;
    //    }
    //    // Return false if not found
    //    return false;
    //}
}
