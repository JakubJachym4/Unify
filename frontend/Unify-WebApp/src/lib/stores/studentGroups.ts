import type { StudentGroup } from "$lib/types/universityClasses";
import { writable } from "svelte/store";

function createStudentGroupStore() {
  const { subscribe, set, update } = writable<StudentGroup[]>([]);

  return {
    subscribe,
    set,
    update,
    addStudentGroup: (studentGroup: StudentGroup) =>
      update((studentGroups) => [...studentGroups, studentGroup]),
    removeStudentGroup: (studentGroup: StudentGroup) =>
      update((studentGroups) =>
        studentGroups.filter((group) => group.id !== studentGroup.id)
      ),
  };
}

export const studentGroupsStore = createStudentGroupStore();